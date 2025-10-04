namespace Core;
using Tool.Interface;
using Godot;
using System.Collections.Generic;
/// <summary>
/// A tool for handling tiling of scene map elements. Has to be a Node to be able to affect other Nodes in the scene tree.
/// </summary>
public sealed partial class TilingTool : Node, ITilingTool
{
	private TileMapLayer _foregroundLayer;
	private TileMapLayer _backgroundLayer;
	private Rect2 _usedRect;
	private Rect2 _worldRect;
	private float _width;
	private float _height;
	private readonly Dictionary<string, (TileMapLayer background, TileMapLayer foreground)> _chunks = new();
	public TilingTool(TileMapLayer foreground, TileMapLayer background)
	{
		_foregroundLayer = foreground;
		_backgroundLayer = background;
	}
	public Rect2 GetWorldRect() => _worldRect;
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
		for (int x = -1; x < 2; x++)
		{
			for (int y = -1; y < 2; y++)
			{
				if (x == 0 && y == 0) continue;
				var backgroundDuplicate = _backgroundLayer.Duplicate() as TileMapLayer;
				var foregroundDuplicate = _foregroundLayer.Duplicate() as TileMapLayer;
				backgroundDuplicate.Position += new Vector2(x * _width, y * _height);
				foregroundDuplicate.Position += new Vector2(x * _width, y * _height);
				if (backgroundDuplicate == null || foregroundDuplicate == null)
				{
					GD.PrintErr("Failed to duplicate TileMapLayer.");
					continue;
				}
				string chunkName = $"{x},{y}";
				_chunks.Add(chunkName, (backgroundDuplicate, foregroundDuplicate));
				GD.Print($"Added chunk {chunkName} at position {backgroundDuplicate.Position}");
				GD.Print($"Added chunk {chunkName} at position {foregroundDuplicate.Position}");
				backgroundDuplicate.Visible = false;
				foregroundDuplicate.Visible = false;
				backgroundDuplicate.Name = $"Background_{chunkName}";
				foregroundDuplicate.Name = $"Foreground_{chunkName}";
				backgroundDuplicate.ZIndex = -999;
				foregroundDuplicate.ZIndex = -998;
				AddChild(backgroundDuplicate);
				AddChild(foregroundDuplicate);
				backgroundDuplicate.Visible = true;
				foregroundDuplicate.Visible = true;
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
		// Update the world rectangle position
		_worldRect.Position += offset;
		// DEBUG
		GD.Print($"World Rect moved to: {_worldRect.Position}");
		GD.Print($"Moving layers due to player crossing {direction} border.");
		_backgroundLayer.Position += offset;
		_foregroundLayer.Position += offset;
		foreach (var chunk in _chunks.Values)
		{
			chunk.background.Position += offset;
			chunk.foreground.Position += offset;
		}
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
