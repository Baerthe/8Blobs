using Godot;
using Core;
using Core.Interface;
using System;
/// <summary>
/// The main class that handles main orchestration and dependency management of the game.
/// </summary>
public partial class Main : Node2D
{
	[ExportGroup("Singles")]
	[ExportSubgroup("Core")]
	[Export] public MenuManager Menu { get; private set; }
	[Export] public Camera2D Camera { get; private set; }
	[Export] public UiManager Ui { get; private set; }
	// Core Orchestration Variables
	private readonly IAudioService _audioService = CoreProvider.GetAudioService();
	private readonly ISaveService _saveService = CoreProvider.GetSaveService();
	private readonly ILevelService _levelService = CoreProvider.GetLevelService();
	private readonly IClockService _clockService = CoreProvider.GetClockService();
	private readonly IDataService _DataService = CoreProvider.GetDataService();
	// Flags and States
	private State CurrentState { get; set; } = State.Menu;
	private bool _isGameOver = false;
	private bool _isGameStarted = false;
	// Engine Callbacks
	public override void _Ready()
	{
		// Do we have everything?
		NullCheck();
		Subscribe();
		_clockService.InitGame(this);
		GD.PrintRich("[color=#000][bgcolor=#00ff00]Main node ready. Initializing game...[/bgcolor][/color]");
		GD.PrintRich("[color=#000][bgcolor=#00ff00]Game Initialized.[/bgcolor][/color]");
		Menu.Show();
	}
	public override void _Process(double delta)
	{
	}
	// Initialization Helpers
	private void NullCheck()
	{
		if (Ui == null)
		{
			GD.PrintErr("UI node not set in Main");
			throw new InvalidOperationException("ERROR 201: UI node not set in Main. Game cannot load.");
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
	private void Subscribe()
	{
		GD.Print("Subscribing to ClockService events...");
		_clockService.PulseTimeout += OnPulseTimeout;
		_clockService.SlowPulseTimeout += OnSlowPulseTimeout;
		_clockService.MobSpawnTimeout += OnMobSpawnTimeout;
		_clockService.PickupSpawnTimeout += OnPickupSpawnTimeout;
		_clockService.GameTimeout += OnGameTimeout;
		_clockService.StartingTimeout += OnStartingTimeout;
		GD.PrintRich("[color=green]ClockService subscription complete.");
		GD.PrintRich("[color=green]Global Player reference set.");
	}

	// Event Handlers
	// Main is subscribed to both pulse clock events, for batching, and this way we can print debug info, but also this let's us optimize some of the logic by having select onEvents
	// get called by main instead of each individual class listeners. We can also solve race conditions this way, if needed.
	private void OnPulseTimeout()
	{
		GD.PrintRich("[color=#afdd00]Pulse Tick processing...");
		ProcessGameState();
	}
	private void OnSlowPulseTimeout()
	{
		GD.PrintRich("[color=#ffaa00]Slow Pulse Tick processing...");
		if (CurrentState != State.Playing) return;
		ProcessMobLogic();
	}
	///TODO: These should be moved to the level tools. They are level specific after all.
	/// We can have the level tool subscribe to these events when the level loads, and unsubscribe when the level unloads (by deleting the level tool node along side the level loaded).
	private void OnMobSpawnTimeout()
	{
		GD.PrintRich("[color=#00aaff]Mob Spawn Tick processing...");
		if (CurrentState != State.Playing) return;
	}
	private void OnPickupSpawnTimeout()
	{
		GD.PrintRich("[color=#D0a0f2]Pickup Spawn Tick processing...");
		if (CurrentState != State.Playing) return;
	}
	private void OnGameTimeout()
	{
		GD.PrintRich("[color=#1f9fA0]Game Timer processing...");
		if (CurrentState != State.Playing) return;
	}
	private void OnStartingTimeout()
	{
		GD.PrintRich("[color=#ffffff]Starting Timer processing...");
		if (CurrentState != State.Playing) return;
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
	private void ProcessMobLogic()
	{
		GD.Print("Processing mob logic...");
		// Get all mobs in the scene and call their process logic.
		// This assumes mobs are in a group called "Mobs". When called, each mob will handle its own logic and only act if within the current chunk.
		// Mobs out of all loaded chunks for long enough will get teleported into the current chunk by the level tool.
		var mobs = GetTree().GetNodesInGroup("Mobs");
		foreach (var mob in mobs)
		{
			if (mob is Mobs.Mob m)
			{
				m.OnSlowPulseTimeout();
			}
		}
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
