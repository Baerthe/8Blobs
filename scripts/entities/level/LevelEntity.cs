namespace Entities;

using Godot;
using System;
/// <summary>
/// LevelEntity is a Node2D that represents a level in the game. It contains various properties that define the level's characteristics, including its name, description, data, tilemap layers, player spawn point, and various systems for managing chests, mobs, and players.
/// </summary>
[GlobalClass]
public partial class LevelEntity : Node2D
{
    [ExportCategory("Stats")]
    [ExportGroup("Components")]
    [ExportSubgroup("TileMaps")]
    [Export] public TileMapLayer ForegroundLayer { get; private set; }
    [Export] public TileMapLayer BackgroundLayer { get; private set; }
    [ExportSubgroup("Markers")]
    [Export] public Node2D PlayerSpawn { get; private set; }
    public override void _Ready()
    {
        NullCheck();
        AddToGroup("levels");
    }
    private void NullCheck()
    {
        byte failure = 0;
        if (ForegroundLayer == null) { GD.PrintErr($"ERROR: {this.Name} does not have ForegroundLayer set!"); failure++; }
        if (BackgroundLayer == null) { GD.PrintErr($"ERROR: {this.Name} does not have BackgroundLayer set!"); failure++; }
        if (PlayerSpawn == null) { GD.PrintErr($"ERROR: {this.Name} does not have PlayerSpawn set!"); failure++; }
        if (failure > 0) throw new InvalidOperationException($"{this.Name} has failed null checking with {failure} missing components!");
    }
}