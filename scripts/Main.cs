using Godot;
using Core;
using Game;
using Core.Interface;
using System;
/// <summary>
/// The main class that handles orchestration and dependency management of the game.
/// </summary>
public partial class Main : Node2D
{
	[ExportGroup("Main Nodes")]
	[ExportSubgroup("Core")]
	[Export] public Camera2D MainCamera { get; private set; }
	[Export] public GameManager GameSystems { get; private set; }
	[Export] public MenuManager Menu { get; private set; }
	[Export] public UiManager Hud { get; private set; }
	// Core Orchestration Variables
	private readonly IAudioService _audioService = CoreProvider.GetAudioService();
	private readonly ISaveService _saveService = CoreProvider.GetSaveService();
	private readonly ILevelService _levelService = CoreProvider.GetLevelService();
	private readonly IClockService _clockService = CoreProvider.GetClockService();
	private readonly IDataService _dataService = CoreProvider.GetDataService();
	// Flags and States
	private State CurrentState { get; set; } = State.Menu;
	private bool _isGameStarted = false;
	// Engine Callbacks
	public override void _Ready()
	{
		// Do we have everything?
		NullCheck();
		Subscribe();
		GD.PrintRich("[color=#000][bgcolor=#00ff00]Main node ready. Initializing game...[/bgcolor][/color]");
		GD.PrintRich("[color=#000][bgcolor=#00ff00]Game Initialized.[/bgcolor][/color]");
		Menu.Show();
	}
	// Initialization Helpers
	private void NullCheck()
	{
		if (Hud == null)
		{
			GD.PrintErr("HUD node not set in Main");
			throw new InvalidOperationException("ERROR 201: HUD node not set in Main. Game cannot load.");
		}
		if (MainCamera == null)
		{
			GD.PrintErr("Camera node not set in Main");
			throw new InvalidOperationException("ERROR 202: Camera node not set in Main. Game cannot load.");
		}
		if (Menu == null)
		{
			GD.PrintErr("Menu node not set in Main");
			throw new InvalidOperationException("ERROR 203: Menu node not set in Main. Game cannot load.");
		}
		if (GameSystems == null)
		{
			GD.PrintErr("Game node not set in Main");
			throw new InvalidOperationException("ERROR 204: Game node not set in Main. Game cannot load.");
		}
		GD.Print("We got all of our nodes! NullCheck Complete");
	}
	private void Subscribe()
	{
		GD.Print("Subscribing to ClockService events...");
		_clockService.PulseTimeout += OnPulseTimeout;
		_clockService.SlowPulseTimeout += OnSlowPulseTimeout;
		GD.PrintRich("[color=green]ClockService subscription complete.");
		GD.PrintRich("[color=green]Global Player reference set.");
	}

	// Event Handlers
	// Main is subscribed to both pulse clock events, for batching, and this way we can print debug info.
	private void OnPulseTimeout()
	{
		GD.PrintRich("[color=#afdd00]Pulse Tick processing...");
		ProcessGameState();
	}
	private void OnSlowPulseTimeout()
	{
		GD.PrintRich("[color=#ffaa00]Slow Pulse Tick processing...");
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
				GameSystems.TogglePause();
				break;
			case State.Playing:
				if (!_isGameStarted)
                {
					_clockService.InitGame(this);
					GameSystems.PrepareLevel(_levelService.LevelInstance);
					Menu.Hide();
					Hud.Show();
					_isGameStarted = true;
                }
				break;
			case State.GameOver:
				// Game over; waiting for player to return to menu or restart
				GameSystems.UnloadLevel();
				_levelService.UnloadLevel();
				_clockService.ResetGame();
				break;
			default:
				GD.PrintErr("Unknown game state!");
				throw new InvalidOperationException("ERROR 200: Unknown game state in Main. Game cannot load.");
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
