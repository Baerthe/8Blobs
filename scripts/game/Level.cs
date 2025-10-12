namespace Game;

using Godot;
using Core;
using Core.Interface;
using Game.Interface;

public sealed partial class Level : Node2D
{
    [ExportGroup("Details")]
    [Export] public string LocationName { get; private set; } = "";
    [Export] public LevelType LocationType { get; private set; } = LevelType.Unset;
    [ExportGroup("Components")]
    [ExportSubgroup("Markers")]
    [Export] public Node2D PlayerSpawn { get; private set; }
    [ExportSubgroup("Systems")]
    [Export] public ChestSystem GetChestSystem { get; private set; }
    [Export] public LevelSystem GetLevelSystem { get; private set; }
    [Export] public MapSystem GetMapSystem { get; private set; }
	private readonly IClockService _clockService = CoreProvider.GetClockService();
    private readonly IDataService _dataService = CoreProvider.GetDataService();
    private readonly ILevelService _levelService = CoreProvider.GetLevelService();
    private double _delta;
    public override void _Ready()
    {
        _clockService.PulseTimeout += OnPulseTimeout;
        base._Ready();
    }
    public override void _Process(double delta)
    {
        _delta = delta;
    }
    public override void _ExitTree()
	{
		_clockService.SlowPulseTimeout -= OnSlowPulseTimeout;
        base._ExitTree();
    }
    private void OnPulseTimeout()
    {
        GetLevelSystem.Update();
    }
    private void OnSlowPulseTimeout()
    {
        GetMapSystem.Update();
    }
}