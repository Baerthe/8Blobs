namespace Game;
using Godot;
using Core;
using System;
using Core.Interface;
/// <summary>
/// GameManager is a Node that manages the core systems of the game, including chest, level, map, mob, and player systems. It integrates with core services for clock management, data handling, and level management, and ensures that these systems are updated appropriately during the game's lifecycle.
/// </summary>
[GlobalClass]
public partial class GameManager : Node2D
{
    [ExportGroup("Systems")]
    public ChestSystem CurrentChestSystem { get; private set; }
    public LevelSystem CurrentLevelSystem { get; private set; }
    public MapSystem CurrentMapSystem { get; private set; }
    public MobSystem CurrentMobSystem { get; private set; }
    public PlayerSystem CurrentPlayerSystem { get; private set; }
    private readonly IClockService _clockService = CoreProvider.GetClockService();
    private double _delta;
    private bool _levelLoaded = false;
    private bool _isPaused = false;
    public override void _Ready()
    {
        _clockService.PulseTimeout += OnPulseTimeout;
    }
    public override void _Process(double delta)
    {
        if (!_levelLoaded) return;
        if (_isPaused) return;
        _delta = delta;
    }
    public void TogglePause()
    {
        _isPaused = !_isPaused;
    }
    public void PrepareLevel(Node2D Level)
    {
        if (_levelLoaded)
        {
            GD.PrintErr("Level already loaded in GameManager");
            throw new InvalidOperationException("ERROR 300: Level already loaded in GameManager. Cannot load another level.");
        }
        CurrentChestSystem = new();
        Level.AddChild(CurrentChestSystem);
        CurrentLevelSystem = new();
        Level.AddChild(CurrentLevelSystem);
        CurrentMapSystem = new();
        Level.AddChild(CurrentMapSystem);
        CurrentMobSystem = new();
        Level.AddChild(CurrentMobSystem);
        CurrentPlayerSystem = new();
        Level.AddChild(CurrentPlayerSystem);
        _levelLoaded = true;
    }
    public void UnloadLevel()
    {
        if (!_levelLoaded)
        {
            GD.PrintErr("No level loaded in GameManager to unload");
            return;
        }
        CurrentChestSystem.QueueFree();
        CurrentChestSystem = null;
        CurrentLevelSystem.QueueFree();
        CurrentLevelSystem = null;
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
        CurrentLevelSystem.Update();
    }
    private void OnSlowPulseTimeout()
    {
        if (!_levelLoaded) return;
        if (_isPaused) return;
        CurrentMapSystem.Update();
    }
}