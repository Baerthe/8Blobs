using Godot;
using Core;
using Core.Interface;
using Tool.Interface;
using System.Collections.Generic;
using System.ComponentModel;
using System;
using System.Diagnostics;
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
	// Core Orchestration Variables
	public static IServices ServiceProvider { get; private set; } = new Services();
	public static Player GlobalPlayer { get; private set; }
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
		GlobalPlayer = Player;
		_clockManager = ServiceProvider.CoreContainer.Resolve<IClockManager>();
		_clockManager.PulseTimeout += OnPulseTimeout;
		_clockManager.SlowPulseTimeout += OnSlowPulseTimeout;
		_clockManager.MobSpawnTimeout += OnMobSpawnTimeout;
		_clockManager.PickupSpawnTimeout += OnPickupSpawnTimeout;
		_clockManager.GameTimeout += OnGameTimeout;
		_clockManager.StartingTimeout += OnStartingTimeout;
		GD.PrintRich("[color=#000][bgcolor=#00ff00]Main node ready. Initializing game...[/bgcolor][/color]");
		_clockManager.InitGame(this);
		ITilingTool tilingTool = ServiceProvider.ToolContainer.Resolve<ITilingTool>();

	}
	public override void _Process(double delta)
	{
		_delta = delta;
	}
	private void OnPulseTimeout()
	{
		GD.PrintRich("[color=#afdd00]Pulse Tick processing...");
	}
	private void OnSlowPulseTimeout()
	{
		GD.PrintRich("[color=#ffaa00]Slow Pulse Tick processing...");
		ProcessGameState();
	}
	private void OnMobSpawnTimeout()
	{
		GD.PrintRich("[color=#00aaff]Mob Spawn Tick processing...");
	}
	private void OnPickupSpawnTimeout(){
		GD.PrintRich("[color=#D0a0f2]Pickup Spawn Tick processing...");
	}
	private void OnGameTimeout()
	{
		GD.PrintRich("[color=#1f9fA0]Game Timer processing...");
	}
	private void OnStartingTimeout()
	{
		GD.PrintRich("[color=#ffffff]Starting Timer processing...");
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
