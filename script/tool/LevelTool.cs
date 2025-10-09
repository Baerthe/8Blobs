namespace Tool;
using Godot;
using System;
using Core.Interface;
using Tool.Interface;
using Container;

public partial class LevelTool : Node2D, ILevelTool
{
    public Node2D LoadedLevel { get; set; }
    public Node2D PlayerSpawn { get; set; }
    public Player player { get; private set; }
    public Camera2D camera { get; private set; }
    public Vector2 OffsetBetweenPickupAndPlayer { get; private set; }
    public Vector2 OffsetBetweenMobAndPlayer { get; private set; }
    public Path2D MobPath { get; set; }
    public PathFollow2D MobSpawner { get; set; }
    public Path2D PickupPath { get; set; }
    public PathFollow2D PickupSpawner { get; set; }
    private readonly IClockManager _clockManager;
    public LevelTool()
    {
        _clockManager = CoreBox.GetClockManager();
        _clockManager.SlowPulseTimeout += OnSlowPulseTimeout;
        GD.Print("LevelTool created");
    }
    public override void _Ready()
    {
        player = GetNode<Player>("../Player");
        camera = GetNode<Camera2D>("../Camera2D");
        if (player == null)
        {
            GD.PrintErr("Player node not found in LevelTool");
            throw new InvalidOperationException("ERROR 301: Player node not found in LevelTool. Game cannot load.");
        }
        if (camera == null)
        {
            GD.PrintErr("Camera node not found in LevelTool");
            throw new InvalidOperationException("ERROR 302: Camera node not found in LevelTool. Game cannot load.");
        }
    }
    private void OnSlowPulseTimeout()
    {
        OffsetBetweenPickupAndPlayer = player.Position - PickupPath.Position;
		OffsetBetweenMobAndPlayer = player.Position - MobPath.Position;
    }
}