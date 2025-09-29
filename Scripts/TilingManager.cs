using Godot;
/// <summary>
/// A manager for handling tiling of background elements.
/// </summary>
public partial class TilingManager : Node
{
	[Export] public TileMapLayer ForegroundLayer { get; private set; }
	[Export] public TileMapLayer BackgroundLayer { get; private set; }
	private Rect2I _usedRect;
	private Rect2 _worldRect; // Add this to store world coordinates
	private float _width;
	private float _height;
	public override void _Ready()
	{
		_usedRect = BackgroundLayer.GetUsedRect();
		float tileSize = BackgroundLayer.TileSet.TileSize.X;
		_width = _usedRect.Size.X * tileSize;
		_height = _usedRect.Size.Y * tileSize;
		// Calculate the world rect from tile coordinates
		_worldRect = new Rect2(
			_usedRect.Position.X * tileSize,
			_usedRect.Position.Y * tileSize,
			_width,
			_height
		);
		for (int x = -1; x < 2; x++)
		{
			for (int y = -1; y < 2; y++)
			{
				if (x == 0 && y == 0) continue;
				var fakeTileMaps = BackgroundLayer.Duplicate() as TileMapLayer;
				fakeTileMaps.Position = new Vector2(x * _width, y * _height);
				AddChild(fakeTileMaps);
				fakeTileMaps = ForegroundLayer.Duplicate() as TileMapLayer;
				fakeTileMaps.Position = new Vector2(x * _width, y * _height);
				AddChild(fakeTileMaps);
			}
		}
	}
	public void Update(Player player)
	{
		if (player == null) return;

		// Now compare with world coordinates
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
