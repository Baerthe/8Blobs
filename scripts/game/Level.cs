namespace Game;
using Godot;
using Core;
using Core.Interface;
public sealed partial class Level : Node2D
{
    [ExportGroup("Details")]
    [Export] public string LocationName { get; private set; } = "";
    [Export] public LevelType LocationType { get; private set; } = LevelType.Unset;
    [ExportGroup("Components")]
    [ExportSubgroup("Markers")]
    [Export] public Node2D PlayerSpawn { get; private set; }
    [ExportSubgroup("Systems")]
    private ChestSystem _chestSystem;
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