using Godot;
using Core;
using Core.Interface;
using Tool.Interface;
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
	private IClockManager _clockManager;
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
		_clockManager = ServiceProvider.CoreContainer.Resolve<IClockManager>();
		_clockManager.PulseTimeout += OnPulseTimeout;
		_clockManager.SlowPulseTimeout += OnSlowPulseTimeout;
		GD.Print("Main node ready.");
		_clockManager.InitGame(this);
		ITilingTool tilingTool = ServiceProvider.ToolContainer.Resolve<TilingTool>();
	}
	public override void _Process(double delta)
	{
		_delta = delta;
	}
	private void OnPulseTimeout()
	{
		GD.Print("Pulse Tick processing...");

	}
	private void OnSlowPulseTimeout()
	{
		GD.Print("Slow Pulse Tick processing...");
	}
	private void ProcessGameState()
	{
		switch (CurrentState)
		{
			case State.Menu:
				// Waiting for player to start game
				break;
			case State.LevelSelect:
				// Waiting for player to select level
				break;
			case State.Paused:
				// Game is paused; waiting for player to unpause
				break;
			case State.Playing:
				if (!_isGameStarted)
				{
				}
				if (_isGameOver)
				{
				}
				break;
			case State.GameOver:
				// Game over; waiting for player to return to menu or restart
				break;
			default:
				GD.PrintErr("Unknown game state!");
				throw new InvalidOperationException("ERROR 200: Unknown game state in Main. Game cannot load.");
		}
	}
	// Utility methods
	private void NullCheck()
	{
		if (Player == null)
		{
			GD.PrintErr("Player node not set in Main");
			throw new InvalidOperationException("ERROR 201: Player node not set in Main. Game cannot load.");
		}
		if (Camera == null)
		{
			GD.PrintErr("Camera node not set in Main");
			throw new InvalidOperationException("ERROR 202: Camera node not set in Main. Game cannot load.");
		}
		if (Menu == null)
		{
			GD.PrintErr("Menu node not set in Main");
			throw new InvalidOperationException("ERROR 203: Menu node not set in Main. Game cannot load.");
		}
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
