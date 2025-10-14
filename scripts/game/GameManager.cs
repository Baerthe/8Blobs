namespace Game;
using Godot;
using Core;
using Core.Interface;
/// <summary>
/// GameManager is a Node that manages the core systems of the game, including chest, level, map, mob, and player systems. It integrates with core services for clock management, data handling, and level management, and ensures that these systems are updated appropriately during the game's lifecycle.
/// </summary>
public partial class GameManager : Node
{
    [ExportGroup("Systems")]
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