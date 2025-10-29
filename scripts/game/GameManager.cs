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
    public bool IsPaused { get; private set; } = false;
    public bool IsLevelLoaded { get; private set; } = false;
    private LevelEntity _levelInstance;
    private LevelData _levelData;
    private IAudioService _audioService;
    private IEventService _eventService;
    private IHeroService _heroService;
    private IPrefService _prefService;
    private ILevelService _levelService;
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
        if (!IsLevelLoaded) return;
        if (IsPaused) return;
    }
    /// <summary>
    /// Toggles the paused state of the game. When paused, it stops processing for the player and mob systems.
    /// </summary>
    public void TogglePause()
    {
        IsPaused = !IsPaused;
        if (IsPaused)
        {
            GetTree().Paused = true;
            CurrentClockSystem.PauseTimers();
        }
        else
        {
            GetTree().Paused = false;
            CurrentClockSystem.ResumeTimers();
        }
    }
    /// <summary>
    /// Prepares the level by initializing and adding the core systems to the provided level node.
    /// </summary>
    /// <param name="Level"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void PrepareLevel()
    {
        if (IsLevelLoaded)
        {
            GD.PrintErr("Level already loaded in GameManager");
            throw new InvalidOperationException("ERROR 300: Level already loaded in GameManager. Cannot load another level.");
        }
        //TODO: We need to replace these resource loads with proper preloading via loadthreading; IE add a loading screen lol.
        // Load level data and instantiate level entity
        _levelData = _levelService.CurrentLevel;
        _levelInstance = ResourceLoader.Load<PackedScene>(_levelData.Entity.ResourcePath).Instantiate<LevelEntity>();
        AddChild(_levelInstance);
        _levelInstance.AddToGroup("level");
        // Initialize and add core systems
        CurrentClockSystem = new(_eventService);
        CurrentChestSystem = new(ResourceLoader.Load<PackedScene>(Templates.ChestTemplate.ResourcePath), _audioService, _eventService);
        CurrentMapSystem = new(_eventService);
        CurrentMobSystem = new(ResourceLoader.Load<PackedScene>(Templates.MobTemplate.ResourcePath), _audioService, _eventService);
        CurrentPlayerSystem = new(ResourceLoader.Load<PackedScene>(Templates.HeroTemplate.ResourcePath), _audioService, _eventService, _heroService);
        // Add systems to level entity
        _levelInstance.AddChild(CurrentClockSystem);
        _levelInstance.AddChild(CurrentChestSystem);
        _levelInstance.AddChild(CurrentMapSystem);
        _levelInstance.AddChild(CurrentMobSystem);
        _levelInstance.AddChild(CurrentPlayerSystem);
        // Initialize systems
        _eventService.Publish<Init>(new Init());
        IsLevelLoaded = true;
    }
    /// <summary>
    /// Unloads the current level by freeing all core systems and resetting the level loaded state.
    /// </summary>
    public void UnloadLevel()
    {
        if (!IsLevelLoaded)
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
        IsLevelLoaded = false;
    }
    private void OnIndexEvent(IEvent eventData) => Templates = ((IndexEvent)eventData).Templates;
}