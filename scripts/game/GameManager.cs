namespace Game;
using Godot;
using Core;
using Entities;
using System;
using Core.Interface;

[GlobalClass]
public partial class GameManager : Node2D
{
    public ClockSystem CurrentClockSystem { get; private set; }
    public ChestSystem CurrentChestSystem { get; private set; }
    public MapSystem CurrentMapSystem { get; private set; }
    public MobSystem CurrentMobSystem { get; private set; }
    public PlayerSystem CurrentPlayerSystem { get; private set; }
    public LevelData CurrentLevelData { get; private set; }
    public Camera2D Camera { get; private set; }
    public EntityIndex Templates { get; private set; }
    public bool IsPaused => _isPaused;
    private LevelEntity _levelInstance;
    private LevelData _levelData;
    private IAudioService _audioService;
    private IEventService _eventService;
    private IHeroService _heroService;
    private IPrefService _prefService;
    private ILevelService _levelService;
    private bool _levelLoaded = false;
    private bool _isPaused = false;
    public override void _Ready()
    {
        _audioService = CoreProvider.AudioService();
        _eventService = CoreProvider.EventService();
        _heroService = CoreProvider.HeroService();
        _prefService = CoreProvider.PrefService();
        _levelService = CoreProvider.LevelService();
        _eventService.Subscribe<IndexEvent>(OnIndexEvent);
        Camera = GetParent().GetNode<Camera2D>("MainCamera");
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
            CurrentClockSystem.PauseTimers();
            CurrentMobSystem.PauseMobs();
        }
        else
        {
            GetTree().Paused = false;
            CurrentClockSystem.ResumeTimers();
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
        _levelInstance.AddToGroup("level");
        // Initialize and add core systems
        CurrentClockSystem = new();
        CurrentChestSystem = new();
        CurrentMapSystem = new();
        CurrentMobSystem = new();
        CurrentPlayerSystem = new();
        // Add systems to level entity
        _levelInstance.AddChild(CurrentClockSystem);
        _levelInstance.AddChild(CurrentChestSystem);
        _levelInstance.AddChild(CurrentMapSystem);
        _levelInstance.AddChild(CurrentMobSystem);
        _levelInstance.AddChild(CurrentPlayerSystem);
        // Initialize systems
        _eventService.Publish("OnInit");
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
        CurrentClockSystem.QueueFree();
        CurrentClockSystem = null;
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
    private void OnIndexEvent(IEvent eventData) => Templates = ((IndexEvent)eventData).Templates;
}