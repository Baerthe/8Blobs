namespace Entities;
using Godot;
/// <summary>
/// LevelEntity is a Node2D that represents a level in the game. It contains various properties that define the level's characteristics, including its name, description, data, tilemap layers, player spawn point, and various systems for managing chests, mobs, and players.
/// </summary>
[GlobalClass]
public partial class LevelEntity : Node2D
{
    [ExportGroup("Details")]
    [Export] public string LevelName { get; private set; } = "";
    [Export] public string Description { get; private set; } = "";
    [ExportGroup("Components")]
    [Export] public LevelData Data { get; private set; }
    [ExportSubgroup("TileMaps")]
    [Export] public TileMapLayer ForegroundLayer { get; private set; }
    [Export] public TileMapLayer BackgroundLayer { get; private set; }
    [ExportSubgroup("Markers")]
    [Export] public Node2D PlayerSpawn { get; private set; }
}