namespace Mobs;

using Core.Interface;
using Core;
using Godot;
/// <summary>
/// A mob is a mobile enemy that moves around the screen and can collide with the player. This is a base class for all mobs.
/// </summary>
public abstract partial class Mob : RigidBody2D
{
    [ExportCategory("Statistics")]
    [Export] public MobMovement MovementType { get; private set; } = MobMovement.CurvedDirection;
    [Export] public byte Health { get; set; } = 1;
    [Export] public byte ExpWorth { get; set; } = 1;
    [Export] public byte Speed { get; set; } = 100;
    [ExportCategory("Parts")]
    [Export] private AnimatedSprite2D _sprite2D;
    [Export] private CollisionShape2D _collision2D;
    [Export] private VisibleOnScreenNotifier2D _notifier2D;
    private bool _ifOffScreen = false;
    private Player _player = Main.GlobalPlayer;
    private readonly IClockManager _clockManager = Main.ServiceProvider.CoreContainer.Resolve<IClockManager>();
    public override void _Ready()
    {
        if (_sprite2D == null) GD.PrintErr("Mob: Sprite2D is null.");
        if (_collision2D == null) GD.PrintErr("Mob: Collision2D is null.");
        if (_notifier2D == null) GD.PrintErr("Mob: Notifier2D is null.");
        _clockManager.SlowPulseTimeout += OnSlowPulseTimeout;
        _sprite2D.Animation = "Walk";
    }
    public override void _Process(double delta)
    {
        _sprite2D.Play();
    }
    public void OnSlowPulseTimeout()
    {
        if (_player == null) return;
        Vector2 directionToPlayer = (_player.Position - GlobalPosition).Normalized();
        if (MovementType == MobMovement.PlayerAttracted)
        {
            LinearVelocity = directionToPlayer * Speed;
            return;
        }
        else if (MovementType == MobMovement.RandomDirection)
        {
            LinearVelocity = LinearVelocity.Rotated((float)GD.RandRange(-0.1, 0.1));
            return;
        }
        else
        {
            directionToPlayer = directionToPlayer.Rotated((float)GD.RandRange(-0.05, 0.05));
            LinearVelocity = LinearVelocity * 0.95f + directionToPlayer * Speed;
        }
    }
    public void Spawn(Vector2 playerPosition, PathFollow2D spawner)
    {
        Position = spawner.GlobalPosition;
        float mobSpeedModifier = Speed + (int)(GD.RandRange(-1.0, 0.5) % Speed);
        float randomAngle = (float)GD.RandRange(-0.2, 0.2);
        if (MovementType == MobMovement.PlayerAttracted)
        {
            Vector2 directionToPlayer = (playerPosition - spawner.GlobalPosition).Normalized();
            directionToPlayer = directionToPlayer.Rotated(randomAngle);
            LinearVelocity = directionToPlayer * mobSpeedModifier;
            return;
        }
        float direction = spawner.Rotation + Mathf.Pi / 2;
        if (MovementType == MobMovement.RandomDirection)
        {
            direction += randomAngle;
        }
        var velocity = new Vector2((float)GD.RandRange(150.0, 250.0), 0);
        var mobBaseSpeed = (Speed + (int)(GD.RandRange(-1.0, 0.5) % Speed)) * new Vector2(1, 0).Rotated(direction);
        LinearVelocity = velocity.Rotated(direction) + mobBaseSpeed;
    }
    public enum MobMovement : byte
    {
        CurvedDirection,
        PlayerAttracted,
        RandomDirection
    }
    private async void Death()
    {
        _sprite2D.Animation = "Death";
        _collision2D.Disabled = true;
        await ToSignal(_sprite2D, "animation_finished");
        QueueFree();
    }
}