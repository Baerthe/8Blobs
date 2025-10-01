using Godot;
using System;
using System.Collections.Generic;
/// <summary>
/// A manager for handling tiling of scene map elements. Has to be a Node to be able to affect other Nodes in the scene tree.
/// TODO: No longer move the player, but rather the world around them, chunky style.
/// </summary>
public sealed partial class TilingManager : Node
{
	private TileMapLayer _foregroundLayer;
	private TileMapLayer _backgroundLayer;
	private Rect2 _usedRect;
	private Rect2 _worldRect;
	private float _width;
	private float _height;
	private readonly Dictionary<string, (TileMapLayer, TileMapLayer)> _chunks = new();
	public void SetTileMapLayers(TileMapLayer foreground, TileMapLayer background)
	{
		_foregroundLayer = foreground;
		_backgroundLayer = background;
	}
	public void LoadTiles()
	{
		if (_foregroundLayer == null || _backgroundLayer == null)
		{
			GD.PrintErr("ForegroundLayer or BackgroundLayer not set. Attempting to find in Tree.");
			_foregroundLayer = GetNode<TileMapLayer>("ForegroundLayer");
			_backgroundLayer = GetNode<TileMapLayer>("BackgroundLayer");
			if (_foregroundLayer == null || _backgroundLayer == null)
			{
				GD.PrintErr("ForegroundLayer or BackgroundLayer not found in Tree.");
				return;
			}
		}
		_usedRect = _backgroundLayer.GetUsedRect();
		float tileSize = _backgroundLayer.TileSet.TileSize.X;
		_width = _usedRect.Size.X * tileSize;
		_height = _usedRect.Size.Y * tileSize;
		_worldRect = new Rect2(
			0, 0,
			_width,
			_height
		);
		// Duplicate the layers 8 times, placing them around the original, add them into the chunks list, as well as the originals.
		for (int x = -1; x < 2; x++)
		{
			for (int y = -1; y < 2; y++)
			{
				if (x == 0 && y == 0)
				{
					_chunks.Add("C", (_backgroundLayer, _foregroundLayer));
				}
				var backgroundDuplicate = _backgroundLayer.Duplicate() as TileMapLayer;
				var foregroundDuplicate = _foregroundLayer.Duplicate() as TileMapLayer;
				backgroundDuplicate.Position = new Vector2(x * _width, y * _height);
				foregroundDuplicate.Position = new Vector2(x * _width, y * _height);
				if (backgroundDuplicate == null || foregroundDuplicate == null)
				{
					GD.PrintErr("Failed to duplicate TileMapLayer.");
					continue;
				}
				string chunkName = GetChunkName(x, y);
				_chunks.Add(chunkName, (backgroundDuplicate, foregroundDuplicate));
				AddChild(backgroundDuplicate);
				AddChild(foregroundDuplicate);
			}
		}
	}
	public void PlayerCrossedBorder(Player player)
	{
		if (player == null || _worldRect.HasPoint(player.Position)) return;

		Direction direction = GetBorderDirection(player.Position);
		if (direction == Direction.OutOfBounds) return;
		MoveLayers(direction);
	}

	private Direction GetBorderDirection(Vector2 playerPosition)
	{
		bool isWest = playerPosition.X < _worldRect.Position.X;
		bool isEast = playerPosition.X > _worldRect.End.X;
		bool isNorth = playerPosition.Y < _worldRect.Position.Y;
		bool isSouth = playerPosition.Y > _worldRect.End.Y;

		if (isNorth && isWest) return Direction.NorthWest;
		if (isNorth && isEast) return Direction.NorthEast;
		if (isSouth && isWest) return Direction.SouthWest;
		if (isSouth && isEast) return Direction.SouthEast;
		if (isWest) return Direction.West;
		if (isEast) return Direction.East;
		if (isNorth) return Direction.North;
		if (isSouth) return Direction.South;

		GD.PrintErr("Player is outside world rect but no direction matched. How they do that??? TilingManager cannot find the direction and has lost the player.");
		return Direction.OutOfBounds;
	}
	private void MoveLayers(Direction direction)
	{
		// Identify which chunks are now outside the visible area
		// Reposition only those specific chunks to the opposite side
		// Maintain the illusion of continuous terrain
		Vector2 offset = direction switch
		{
			Direction.NorthWest => new Vector2(-_width, -_height),
			Direction.North => new Vector2(0, -_height),
			Direction.NorthEast => new Vector2(_width, -_height),
			Direction.West => new Vector2(-_width, 0),
			Direction.East => new Vector2(_width, 0),
			Direction.SouthWest => new Vector2(-_width, _height),
			Direction.South => new Vector2(0, _height),
			Direction.SouthEast => new Vector2(_width, _height),
			_ => Vector2.Zero
		};
		List<(TileMapLayer, TileMapLayer)> layersToMove = new();
		foreach (var (chunkName, (background, foreground)) in _chunks)
		{
			bool shouldMove = direction switch
			{
				Direction.North => chunkName.Contains("S"),
				Direction.South => chunkName.Contains("N"),
				Direction.West => chunkName.Contains("E"),
				Direction.East => chunkName.Contains("W"),
				Direction.NorthWest => chunkName.Contains("E") || chunkName.Contains("S"),
				Direction.NorthEast => chunkName.Contains("W") || chunkName.Contains("S"),
				Direction.SouthWest => chunkName.Contains("E") || chunkName.Contains("N"),
				Direction.SouthEast => chunkName.Contains("W") || chunkName.Contains("N"),
				_ => false
			};
			if (shouldMove)
			{
				layersToMove.Add((background, foreground));
				// Rename the chunk to reflect its new position
				var (newX, newY) = GetNewChunkCoordinates(chunkName, direction);
				string newChunkName = GetChunkName(newX, newY);
				_chunks.Remove(chunkName);
				_chunks.Add(newChunkName, (background, foreground));
			}
		}
		// Update the world rectangle position
		_worldRect.Position += offset;
		// DEBUG
		GD.Print($"World Rect moved to: {_worldRect.Position}");
		GD.Print($"Moving {layersToMove.Count} layers due to player crossing {direction} border.");
		// Move the identified layers
		foreach (var (background, foreground) in layersToMove)
		{
			background.Position += offset;
			foreground.Position += offset;
		}
	}
	private string GetChunkName(int x, int y)
	{
		string chunkName = "";
		if (y == -1) chunkName += "N";
		if (y == 1) chunkName += "S";
		if (x == 1) chunkName += "E";
		if (x == -1) chunkName += "W";
		return chunkName;
	}
	private (int, int) GetNewChunkCoordinates(string chunkName, Direction direction)
	{
		int x = 0, y = 0;
		if (chunkName.Contains('N')) y = -1;
		else if (chunkName.Contains('S')) y = 1;
		if (chunkName.Contains('W')) x = -1;
		else if (chunkName.Contains('E')) x = 1;

		switch (direction)
		{
			case Direction.North:
				y -= 2;
				break;
			case Direction.South:
				y += 2;
				break;
			case Direction.West:
				x -= 2;
				break;
			case Direction.East:
				x += 2;
				break;
		}
		// Clamp to -1, 0, 1
		x = Math.Max(-1, Math.Min(1, x));
		y = Math.Max(-1, Math.Min(1, y));
		return (x, y);
	}
	private enum Direction : byte
	{
		NorthWest,
		North,
		NorthEast,
		West,
		East,
		SouthWest,
		South,
		SouthEast,
		OutOfBounds
	}
}
