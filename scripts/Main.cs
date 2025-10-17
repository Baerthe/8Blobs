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
	[Export] public GameManager GameManagerInstance { get; private set; }
	[Export] public MenuManager MenuManagerInstance { get; private set; }
	[Export] public UiManager HudManagerInstance { get; private set; }
	// State
	public static State CurrentState { get; set; } = State.Menu;
	private State _priorState;
	// Core Orchestration Variables
	private readonly IAudioService _audioService = CoreProvider.GetAudioService();
	private readonly ILevelService _levelService = CoreProvider.GetLevelService();
	private readonly IClockService _clockService = CoreProvider.GetClockService();
	// Flags
	private bool _isGameStarted = false;
	// State Enum
	public enum State
	{
		Menu,
		LevelSelect,
		Paused,
		Playing,
		GameOver
	}
	// Engine Callbacks
	public override void _Ready()
	{
		NullCheck();
		Subscribe();
		GD.PrintRich("[color=#000][bgcolor=#00ff00]Main node ready. Initializing game...[/bgcolor][/color]");
		GD.PrintRich("[color=#000][bgcolor=#00ff00]Game Initialized.[/bgcolor][/color]");
		CurrentState = State.Menu;
		_priorState = CurrentState;
	}
	/// <summary>
    /// Validates that all critical nodes are assigned in the editor. If any are missing, it logs an error and throws an exception to prevent the game from running in an invalid state.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
	private void NullCheck()
	{
		if (HudManagerInstance == null)
		{
			GD.PrintErr("HUD node not set in Main");
			throw new InvalidOperationException("ERROR 201: HUD node not set in Main. Game cannot load.");
		}
		if (MainCamera == null)
		{
			GD.PrintErr("Camera node not set in Main");
			throw new InvalidOperationException("ERROR 202: Camera node not set in Main. Game cannot load.");
		}
		if (MenuManagerInstance == null)
		{
			GD.PrintErr("Menu node not set in Main");
			throw new InvalidOperationException("ERROR 203: Menu node not set in Main. Game cannot load.");
		}
		if (GameManagerInstance == null)
		{
			GD.PrintErr("Game node not set in Main");
			throw new InvalidOperationException("ERROR 204: Game node not set in Main. Game cannot load.");
		}
		GD.Print("We got all of our nodes! NullCheck Complete");
	}
	/// <summary>
    /// Subscribes to ClockService events for pulse and slow pulse ticks. This is where we drive the main game loop by processing game state changes on each tick.
    /// </summary>
	private void Subscribe()
	{
		GD.Print("Subscribing to ClockService events...");
		_clockService.PulseTimeout += OnPulseTimeout;
		_clockService.SlowPulseTimeout += OnSlowPulseTimeout;
		GD.PrintRich("[color=green]ClockService subscription complete.");
		GD.PrintRich("[color=green]Global Player reference set.");
	}
	/// <summary>
    /// Handles regular pulse ticks from the ClockService to update game state.
    /// </summary>
	private void OnPulseTimeout()
	{
		GD.PrintRich("[color=#afdd00]Pulse Tick processing...");
		ProcessGameState();
	}
	private void OnSlowPulseTimeout()
	{
		GD.PrintRich("[color=#ffaa00]Slow Pulse Tick processing...");
	}
	/// <summary>
    /// Handles transitions between different game states. CurrentState is static and can be changed from anywhere, it is checked every pulse tick; this allows for decoupled state changes that intrrupt the game flow.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
	private void ProcessGameState()
	{
		if (_priorState == CurrentState) return;
		switch (CurrentState)
		{
			case State.Menu:
				// Waiting for player to start game
				MenuManagerInstance.Show();
				break;
			case State.LevelSelect:
				// Waiting for player to select level
				break;
			case State.Paused:
				// Game is paused; waiting for player to unpause
				GameManagerInstance.TogglePause();
				break;
			case State.Playing:
				if (GameManagerInstance.IsPaused)
				{
					GameManagerInstance.TogglePause();
				}
				if (!_isGameStarted)
				{
					_clockService.InitGame(this);
					GameManagerInstance.PrepareLevel(_levelService.LevelInstance);
					MenuManagerInstance.Hide();
					HudManagerInstance.Show();
					_isGameStarted = true;
				}
				break;
			case State.GameOver:
				// Game over; waiting for player to return to menu or restart
				GameManagerInstance.UnloadLevel();
				_levelService.UnloadLevel();
				_clockService.ResetGame();
				break;
			default:
				GD.PrintErr("Unknown game state!");
				throw new InvalidOperationException("ERROR 200: Unknown game state in Main. Game cannot load.");
		}
		_priorState = CurrentState;
	}
}
