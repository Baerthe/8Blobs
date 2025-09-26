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
    [Export] private AnimatedSprite2D _sprite2D;
    [Export] private CollisionShape2D _collision2D;
    [Export] private VisibleOnScreenNotifier2D _notifier2D;
    public override void _Ready()
    {
        _sprite2D.Animation = "Walk";
    }
    public override void _Process(double delta)
    {
        _sprite2D.Play();
        // Handle movement based on type
        switch (MovementType)
        {
            case MobMovement.LinearDirection:
                // Already handled by physics engine
                break;
            case MobMovement.PlayerAttracted:
                // Move towards player (not implemented)
                break;
            case MobMovement.RandomDirection:
                // Change direction randomly (not implemented)
                break;
            default:
                break;
        }
    }
    public enum MobMovement : byte
    {
        LinearDirection,
        PlayerAttracted,
        RandomDirection
    }
    private void OnSceneExit()
    {
        if (LinearVelocity.Length() < 10)
            Death();
    }
    private void Death()
    {
        _sprite2D.Animation = "Death";
        _collision2D.Disabled = true;
        RemoveAfterAnimation();
    }
    private async void RemoveAfterAnimation()
    {
        await ToSignal(_sprite2D, "animation_finished");
        QueueFree();
    }
}