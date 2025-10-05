using Godot;
using Core;
using Core.Interface;
using System.Collections.Generic;
using System.ComponentModel;
using System;
/// <summary>
/// The main class that handles main orchestration and dependency management of the game.
/// </summary>
public partial class Main : Node2D
{
	[ExportGroup("Singles")]
	[ExportSubgroup("Core")]
	[Description("Core nodes for handling game logic and tiling.")]
	[Export] public Menu Menu { get; private set; }
	[Export] public Player Player { get; private set; }
	[Export] public Marker2D PlayerStart { get; private set; }
	[Export] public Camera2D Camera { get; private set; }
	[Export] public Ui Ui { get; private set; }
	//////// These need to be moved vvvvvvvvvv
	/// We need to move these level specific variables out of Main and into level specific managers or data files.
	/// They are here for now to avoid hardcoding values in the managers; for testing.
	/// They are level specific and should be set in the level scene or a level data file
	//////// These need to be moved vvvvvvvvvv
	[ExportSubgroup("Timers")]
	[Description("The amount of time, in seconds, for spawning of mobs, pickups, score increment, and game start countdown.")]
	[Export] public float MobSpawnTime { get; private set; } = 1.5f;
	[Export] public float PickupSpawnTime { get; private set; } = 5f;
	[Export] public float ScoreTime { get; private set; } = 1f;
	[Export] public float StartingTime { get; private set; } = 3f;
	// Spawners
	[ExportGroup("Spawnables")]
	[Description("Scenes and paths for spawning mobs and pickups.")]
	[ExportSubgroup("Mobs")]
	[Export] public PackedScene[] MobScenes { get; private set; }
	[Export] private Path2D _mobPath;
	[Export] private PathFollow2D _mobSpawner;
	[ExportSubgroup("Pickups")]
	[Export] public PackedScene[] PickupScenes { get; private set; }
	[Export] private Path2D _pickupPath;
	[Export] private PathFollow2D _pickupSpawner;
	//////// These need to be moved ^^^^^^^^^^
	//////// These need to be moved ^^^^^^^^^^
	// Non-node Core Orchestration Variables
	public static IServices ServiceProvider { get; private set; } = new Services();
	// Flags and States
	private State CurrentState { get; set; } = State.Menu;
	private bool _isGameOver = false;
	private bool _isGameStarted = false;
	private double _delta;
	// Engine Callbacks
	public override void _Ready()
	{
		// Do we have everything?
		NullCheck();
		GD.Print("Main node ready.");
	}
	public override void _Process(double delta)
	{
		_delta = delta;
	}
	// Utility methods
	private void NullCheck()
	{
		if (Player == null) GD.PrintErr("Player node not set in Main");
		if (PlayerStart == null) GD.PrintErr("PlayerStart node not set in Main");
		if (Camera == null) GD.PrintErr("Camera node not set in Main");
		if (Ui == null) GD.PrintErr("Ui node not set in Main");
		if (MobScenes == null || MobScenes.Length == 0) GD.PrintErr("MobScenes not set in Main");
		if (_mobPath == null) GD.PrintErr("MobPath node not set in Main");
		if (_mobSpawner == null) GD.PrintErr("MobSpawner node not set in Main");
		if (PickupScenes == null || PickupScenes.Length == 0) GD.PrintErr("PickupScenes not set in Main");
		if (_pickupPath == null) GD.PrintErr("PickupPath node not set in Main");
		if (_pickupSpawner == null) GD.PrintErr("PickupSpawner node not set in Main");
		if (Menu == null) GD.PrintErr("Menu node not set in Main");
		GD.Print("We got all of our nodes! NullCheck Complete");
	}
	private enum State
	{
		Menu,
		LevelSelect,
		Paused,
		Playing,
		GameOver
	}
}
