using Godot;
using Core;
using Game;
using Entities;
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
	[Export] public GameManager GameManager { get; private set; }
	[Export] public MenuManager MenuManager { get; private set; }
	[Export] public UiManager UiManager { get; private set; }
	[ExportGroup("Indices")]
	[Export] public HeroIndex Heroes { get; private set; }
	[Export] public EntityIndex Templates { get; private set; }
	[Export] public ItemIndex Items { get; private set; }
	[Export] public LevelIndex Levels { get; private set; }
	[Export] public WeaponIndex Weapons { get; private set; }
	// Events
	private IEvent _indexEvent;
	// State
	public static State CurrentState { get; set; } = State.Menu;
	private State _priorState;
	// Services
	private IEventService _eventService;
	private ILevelService _levelService;
	// Flags
	private bool _isGameStarted = false;
	// State Enum
	public enum State
	{
		Menu, LevelSelect, Paused, Playing, GameOver
	}
	// Engine Callbacks
	public override void _Ready()
	{
		NullCheck();
		GD.PrintRich("[color=#000][bgcolor=#00ff00]Main node ready. Initializing game...[/bgcolor][/color]");
		_eventService = CoreProvider.EventService();
		_levelService = CoreProvider.LevelService();
		GD.PrintRich("[color=#000][bgcolor=#00ff00]Game Initialized.[/bgcolor][/color]");
		_indexEvent = new IndexEvent(Heroes, Templates, Items, Levels, Weapons);
		CurrentState = State.Menu;
		_priorState = CurrentState;
	}
	public override void _Process(double delta)
	{
		ProcessGameState();
	}
	/// <summary>
	/// Validates that all critical nodes are assigned in the editor. If any are missing, it logs an error and throws an exception to prevent the game from running in an invalid state.
	/// </summary>
	/// <exception cref="InvalidOperationException"></exception>
	private void NullCheck()
	{
		if (UiManager == null) throw new InvalidOperationException("ERROR 201: HUD node not set in Main. Game cannot load.");
		if (MainCamera == null) throw new InvalidOperationException("ERROR 202: Camera node not set in Main. Game cannot load.");
		if (MenuManager == null) throw new InvalidOperationException("ERROR 203: Menu node not set in Main. Game cannot load.");
		if (GameManager == null) throw new InvalidOperationException("ERROR 204: Game node not set in Main. Game cannot load.");
		GD.Print("We got all of our nodes! Checking Indices...");
		if (Heroes == null) throw new InvalidOperationException("ERROR 205: HeroIndex not set in Main. Game cannot load.");
		if (Templates == null) throw new InvalidOperationException("ERROR 206: EntityIndex not set in Main. Game cannot load.");
		if (Items == null) throw new InvalidOperationException("ERROR 207: ItemIndex not set in Main. Game cannot load.");
		if (Levels == null) throw new InvalidOperationException("ERROR 208: LevelIndex not set in Main. Game cannot load.");
		if (Weapons == null) throw new InvalidOperationException("ERROR 209: WeaponIndex not set in Main. Game cannot load.");
		GD.Print("We got all of our indices! NullCheck Complete");
	}
    /// <summary>
    /// Handles transitions between different game states. CurrentState is static and can be changed from anywhere, it is checked every pulse tick; this allows for decoupled state changes that intrrupt the game flow.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
	private void ProcessGameState()
	{
		if (_priorState == CurrentState) return;
		_priorState = CurrentState;
		switch (CurrentState)
		{
			case State.Menu:
				// Waiting for player to start game
				MenuManager.Show();
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
					GameManager.PrepareLevel();
					_eventService.Publish<IEvent>(_indexEvent);
					MenuManager.Hide();
					UiManager.Show();
					_isGameStarted = true;
				}
				break;
			case State.GameOver:
				// Game over; waiting for player to return to menu or restart
				GameManager.UnloadLevel();
				_levelService.UnloadLevel();
				break;
			default:
				GD.PrintErr("Unknown game state!");
				throw new InvalidOperationException("ERROR 200: Unknown game state in Main. Game cannot load.");
		}
		_priorState = CurrentState;
	}
}
