namespace Game;
using Godot;
using Core;
using Entities;
using System;
using Core.Interface;
/// <summary>
/// GameManager is a Node that manages the core systems of the game, including chest, level, map, mob, and player systems. It integrates with core services for clock management, data handling, and level management, and ensures that these systems are updated appropriately during the game's lifecycle.
/// </summary>
[GlobalClass]
public partial class GameManager : Node2D
{
    public event EventHandler<LevelLoadArgs> OnLevelLoad;
    // TODO: Consider adding an OnLevelUnload event for better cleanup?
    // TODO: Consider adding an EventSystem to manage events more effectively, especially between systems.
    public ChestSystem CurrentChestSystem { get; private set; }
    public MapSystem CurrentMapSystem { get; private set; }
    public MobSystem CurrentMobSystem { get; private set; }
    public PlayerSystem CurrentPlayerSystem { get; private set; }
    public LevelData CurrentLevelData { get; private set; }
    public Camera2D Camera { get; private set; }
    public EntityIndex Templates { get; private set; }
    public bool IsPaused => _isPaused;
    private HeroEntity _heroInstance;
    private LevelData _levelData;
    private LevelEntity _levelInstance;
    private readonly IClockService _clockService = CoreProvider.GetClockService();
    private readonly ILevelService _levelService = CoreProvider.GetLevelService();
    private bool _levelLoaded = false;
    private bool _isPaused = false;
    public override void _Ready()
    {
        _clockService.PulseTimeout += OnPulseTimeout;
        _clockService.SlowPulseTimeout += OnSlowPulseTimeout;
        Camera = GetParent().GetNode<Camera2D>("MainCamera");
        Templates = ResourceLoader.Load<EntityIndex>("res://assets/data/indices/EntityIndex.tres");
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
        // Load level data and instantiate level entity
        _levelData = _levelService.CurrentLevel;
        _levelInstance = ResourceLoader.Load<PackedScene>(_levelData.Entity.ResourcePath).Instantiate<LevelEntity>();
        AddChild(_levelInstance);
        // Initialize and add core systems
        CurrentChestSystem = new();
        CurrentMapSystem = new();
        CurrentMobSystem = new();
        CurrentPlayerSystem = new();
        // Add systems to level entity
        _levelInstance.AddChild(CurrentChestSystem);
        _levelInstance.AddChild(CurrentMapSystem);
        _levelInstance.AddChild(CurrentMobSystem);
        _levelInstance.AddChild(CurrentPlayerSystem);
        // Create hero instance
        _heroInstance = CurrentPlayerSystem.PlayerInstance;
        // All of our systems are ready, now initialize them by calling the load event.
        OnLevelLoad?.Invoke(this, new LevelLoadArgs(Templates, _levelInstance, _heroInstance));
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
        _levelInstance.QueueFree();
        _levelData = null;
        _levelLoaded = false;
    }
    private void OnPulseTimeout()
    {
        if (!_levelLoaded) return;
        if (_isPaused) return;
        CurrentPlayerSystem.Update();
        CurrentMapSystem.Update();
        CurrentMobSystem.Update();
    }
    private void OnSlowPulseTimeout()
    {
        if (!_levelLoaded) return;
        if (_isPaused) return;
        CurrentChestSystem.Update();
        CurrentMapSystem.Update();
    }
    public class LevelLoadArgs : EventArgs
    {
        public EntityIndex Templates { get; set; }
        public LevelEntity LevelInstance { get; set; }
        public HeroEntity PlayerInstance { get; set; }
        public LevelLoadArgs(EntityIndex templates, LevelEntity levelInstance, HeroEntity playerInstance)
        {
            Templates = templates;
            LevelInstance = levelInstance;
            PlayerInstance = playerInstance;
        }
    }
}