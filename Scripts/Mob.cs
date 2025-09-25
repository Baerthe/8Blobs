using Godot;
using System;
/// <summary>
/// A mob is a mobile enemy that moves around the screen and can collide with the player. This is a base class for all mobs.
/// </summary>
public abstract partial class Mob : RigidBody2D
{
    [ExportCategory("Statistics")]
    [Export] public MobMovement MovementType { get; private set; } = MobMovement.LinearDirection;
    [Export] public int Speed { get; set; } = 100;
    [ExportCategory("Parts")]
    [Export] public AnimatedSprite2D Sprite2D { get; private set; }
    [Export] public CollisionShape2D Collision2D { get; private set; }
    [Export] public VisibleOnScreenNotifier2D Notifier2D { get; private set; }
    public override void _Ready()
    {
        Sprite2D.Animation = "Walk";
    }
    public override void _Process(double delta)
    {

    }
    public enum MobMovement : byte
    {
        LinearDirection,
        PlayerAttracted,
        RandomDirection
    }
    private void OnSceneExit() => QueueFree();
}