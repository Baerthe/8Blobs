using Godot;
using System;
/// <summary>
/// The main class that handles the game logic, including spawning mobs and managing the game state.
/// </summary>
public partial class Main : Node2D
{
	[ExportGroup("Singles")]
	[Export] public Player Player { get; private set; }
	[Export] public Marker2D PlayerStart { get; private set; }
	[Export] public Camera2D Camera { get; private set; }
	[ExportGroup("Spawnables")]
	[ExportSubgroup("Mobs")]
	[Export] public PackedScene[] MobScenes { get; private set; }
	private int _score = 0;
	public override void _Ready()
	{
		Player.Start(PlayerStart.Position);
	}
	public override void _Process(double delta)
	{
		Camera.Position = Player.Position;
	}
}
