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
    [Export] public LevelSystem GetLevelSystem { get; private set; }
    private readonly ILevelService _levelService = CoreProvider.GetLevelService();
	private readonly IClockService _clockService = CoreProvider.GetClockService();
	private readonly IDataService _DataService = CoreProvider.GetDataService();
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
    private void OnPulseTimeout()
    {
        GetLevelSystem.Update();
    }
    private void OnSlowPulseTimeout()
    {

    }
}