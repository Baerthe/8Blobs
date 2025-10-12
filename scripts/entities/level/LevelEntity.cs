namespace Entities;
using Godot;
using Core;
using Game;
using Core.Interface;
[GlobalClass]
public sealed partial class LevelEntity : Node2D
{
    [ExportGroup("Details")]
    [Export] public string LevelName { get; private set; } = "";
    [Export] public LevelType Location { get; private set; } = LevelType.Unset;
    [Export] public MobIndex SpawnTable { get; private set; }
    [ExportGroup("Components")]
    [ExportSubgroup("TileMaps")]
    [Export] public TileMapLayer ForegroundLayer { get; private set; }
    [Export] public TileMapLayer BackgroundLayer { get; private set; }
    [ExportSubgroup("Markers")]
    [Export] public Node2D PlayerSpawn { get; private set; }
    [ExportSubgroup("Systems")]
    [Export] public ChestSystem CurrentChestSystem { get; private set; }
    [Export] public LevelSystem CurrentLevelSystem { get; private set; }
    [Export] public MapSystem CurrentMapSystem { get; private set; }
    [Export] public MobSystem CurrentMobSystem { get; private set; }
    [Export] public PlayerSystem CurrentPlayerSystem { get; private set; }
    private readonly IClockService _clockService = CoreProvider.GetClockService();
    private readonly IDataService _dataService = CoreProvider.GetDataService();
    private readonly ILevelService _levelService = CoreProvider.GetLevelService();
    private double _delta;
    public override void _Ready()
    {
        _clockService.PulseTimeout += OnPulseTimeout;
    }
    public override void _Process(double delta)
    {
        _delta = delta;
    }
    public override void _ExitTree()
    {
		_clockService.SlowPulseTimeout -= OnSlowPulseTimeout;
    }
    private void OnPulseTimeout()
    {
        CurrentLevelSystem.Update();
    }
    private void OnSlowPulseTimeout()
    {
        CurrentMapSystem.Update();
    }
}