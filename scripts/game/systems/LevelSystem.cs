namespace Game;
using Godot;
using System;
using Game.Interface;
public sealed partial class LevelSystem : Node2D, IGameSystem
{
    public bool IsInitialized { get; private set; } = false;
    public Node2D player { get; private set; } //temp
    public Camera2D camera { get; private set; }
    public Vector2 OffsetBetweenPickupAndPlayer { get; private set; }
    public Vector2 OffsetBetweenMobAndPlayer { get; private set; }
    public Path2D MobPath { get; set; }
    public PathFollow2D MobSpawner { get; set; }
    public Path2D PickupPath { get; set; }
    public PathFollow2D PickupSpawner { get; set; }
    public override void _Ready()
    {
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
    public void Update()
    {
        OffsetBetweenPickupAndPlayer = player.Position - PickupPath.Position;
		OffsetBetweenMobAndPlayer = player.Position - MobPath.Position;
    }
}