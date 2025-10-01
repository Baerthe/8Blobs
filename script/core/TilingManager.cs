using Godot;
using System.Collections.Generic;
/// <summary>
/// A manager for handling tiling of scene map elements. Has to be a Node to be able to affect other Nodes in the scene tree.
/// </summary>
public sealed partial class TilingManager : Node
{
	private TileMapLayer _foregroundLayer;
	private TileMapLayer _backgroundLayer;
	private Rect2 _usedRect;
	private Rect2 _worldRect;
	private float _width;
	private float _height;
	private readonly List<TileMapLayer> _duplicatedLayers = new();
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
		for (int x = -1; x < 2; x++)
		{
			for (int y = -1; y < 2; y++)
			{
				if (x == 0 && y == 0) continue;
				var backgroundDuplicate = _backgroundLayer.Duplicate() as TileMapLayer;
				backgroundDuplicate.Position = new Vector2(x * _width, y * _height);
				AddChild(backgroundDuplicate);
				_duplicatedLayers.Add(backgroundDuplicate);
				var foregroundDuplicate = _foregroundLayer.Duplicate() as TileMapLayer;
				foregroundDuplicate.Position = new Vector2(x * _width, y * _height);
				AddChild(foregroundDuplicate);
				_duplicatedLayers.Add(foregroundDuplicate);
			}
		}
	}
	public override void _ExitTree()
	{
		foreach (var layer in _duplicatedLayers)
		{
			if (IsInstanceValid(layer))
			{
				layer.QueueFree();
			}
		}
		_duplicatedLayers.Clear();
		base._ExitTree();
	}
	public void PlayerCrossedBorder(Player player)
	{
		if (player == null) return;
		if (!_worldRect.HasPoint(player.Position))
		{
			if (player.Position.X < _worldRect.Position.X)
			{
				player.Position += new Vector2(_width, 0);
				MoveContent(new Vector2(_width, 0));
			}
			else if (player.Position.X > _worldRect.End.X)
			{
				player.Position += new Vector2(-_width, 0);
				MoveContent(new Vector2(-_width, 0));
			}
			if (player.Position.Y < _worldRect.Position.Y)
			{
				player.Position += new Vector2(0, _height);
				MoveContent(new Vector2(0, _height));
			}
			else if (player.Position.Y > _worldRect.End.Y)
			{
				player.Position += new Vector2(0, -_height);
				MoveContent(new Vector2(0, -_height));
			}
		}
	}
	private void MoveContent(Vector2 offset)
	{
		GetTree().CallGroup("Mobs", "MoveContent", offset);
		GetTree().CallGroup("Pickups", "MoveContent", offset);
		GetTree().CallGroup("Traps", "MoveContent", offset);
	}
}
