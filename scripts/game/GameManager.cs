namespace Game;
using Godot;
using Core;
using Entities;
using System;
/// <summary>
/// GameManager is a Node that manages the core systems of the game, including chest, level, map, mob, and player systems. It integrates with core services for clock management, data handling, and level management, and ensures that these systems are updated appropriately during the game's lifecycle.
/// </summary>
[GlobalClass]
public partial class GameManager : Node2D
{
    public ChestSystem CurrentChestSystem { get; private set; }
    public MapSystem CurrentMapSystem { get; private set; }
    public MobSystem CurrentMobSystem { get; private set; }
    public PlayerSystem CurrentPlayerSystem { get; private set; }
    public LevelEntity CurrentLevelEntity { get; private set; }
    public bool IsPaused => _isPaused;
    private Camera2D _camera;
    private bool _levelLoaded = false;
    private bool _isPaused = false;
    public override void _Ready()
    {
        _camera = GetParent().GetNode<Camera2D>("MainCamera");
        CoreProvider.GetClockService().PulseTimeout += OnPulseTimeout;
    }
    public override void _Process(double delta)
    {
        if (!_levelLoaded) return;
        if (_isPaused) return;
    }
    /// <summary>
    /// Toggles the paused state of the game. When paused, it stops processing for the player and mob systems.
    /// </summary>
    public void TogglePause()
    {
        _isPaused = !_isPaused;
        if (_isPaused)
        {
            GetTree().Paused = true;
            CurrentPlayerSystem.PlayerInstance?.SetProcess(false);
            CurrentPlayerSystem.PlayerInstance?.SetPhysicsProcess(false);
            CurrentMobSystem.PauseMobs();
        }
        else
        {
            GetTree().Paused = false;
            CurrentPlayerSystem.PlayerInstance?.SetProcess(true);
            CurrentPlayerSystem.PlayerInstance?.SetPhysicsProcess(true);
            CurrentMobSystem.ResumeMobs();
        }
    }
    /// <summary>
    /// Prepares the level by initializing and adding the core systems to the provided level node.
    /// </summary>
    /// <param name="Level"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void PrepareLevel()
    {
        if (_levelLoaded)
        {
            GD.PrintErr("Level already loaded in GameManager");
            throw new InvalidOperationException("ERROR 300: Level already loaded in GameManager. Cannot load another level.");
        }
        CurrentLevelEntity = ResourceLoader.Load<LevelEntity>(CoreProvider.GetLevelService().CurrentLevel.GetPath());
        AddChild(CurrentLevelEntity);
        CurrentChestSystem = new();
        CurrentMapSystem = new();
        CurrentMobSystem = new();
        CurrentPlayerSystem = new();
        CurrentLevelEntity.AddChild(CurrentChestSystem);
        CurrentLevelEntity.AddChild(CurrentMapSystem);
        CurrentLevelEntity.AddChild(CurrentMobSystem);
        CurrentLevelEntity.AddChild(CurrentPlayerSystem);
        CurrentPlayerSystem.LoadPlayer(ResourceLoader.Load<PackedScene>("res://scenes/entities/heros/TestHero.tscn")); // Temporary until we have a proper player selection system
        CurrentMobSystem.PlayerInstance = CurrentPlayerSystem.PlayerInstance;
        CurrentChestSystem.PlayerInstance = CurrentPlayerSystem.PlayerInstance;
        CurrentMobSystem.LevelInstance = CurrentLevelEntity;
        _levelLoaded = true;
    }
    /// <summary>
    /// Unloads the current level by freeing all core systems and resetting the level loaded state.
    /// </summary>
    public void UnloadLevel()
    {
        if (!_levelLoaded)
        {
            GD.PrintErr("No level loaded in GameManager to unload");
            return;
        }
        CurrentChestSystem.QueueFree();
        CurrentChestSystem = null;
        CurrentMapSystem.QueueFree();
        CurrentMapSystem = null;
        CurrentMobSystem.QueueFree();
        CurrentMobSystem = null;
        CurrentPlayerSystem.QueueFree();
        CurrentPlayerSystem = null;
        _levelLoaded = false;
    }
    private void OnPulseTimeout()
    {
        if (!_levelLoaded) return;
        if (_isPaused) return;
        CurrentPlayerSystem.Update();
        CurrentMobSystem.Update();
    }
    private void OnSlowPulseTimeout()
    {
        if (!_levelLoaded) return;
        if (_isPaused) return;
        CurrentChestSystem.Update();
        CurrentMapSystem.Update();
    }
}