namespace Mobs;
using Godot;
/// <summary>
/// A mob is a mobile enemy that moves around the screen and can collide with the player. This is a base class for all mobs.
/// </summary>
public abstract partial class Mob : RigidBody2D
{
    [ExportCategory("Statistics")]
    [Export] public MobMovement MovementType { get; private set; } = MobMovement.CurvedDirection;
    [Export] public int Speed { get; set; } = 100;
    [ExportCategory("Parts")]
    [Export] private AnimatedSprite2D _sprite2D;
    [Export] private CollisionShape2D _collision2D;
    [Export] private VisibleOnScreenNotifier2D _notifier2D;
    private const float _OutOfScreenTimeToDie = 10.0f;
    private bool _ifOffScreen = false;
    public override void _Ready()
    {
        _sprite2D.Animation = "Walk";
    }
    public override void _Process(double delta)
    {
        _sprite2D.Play();
    }
    public void Update(Player player)
    {
        if (player == null) return;
        Vector2 directionToPlayer = (player.Position - GlobalPosition).Normalized();
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
            LinearVelocity = LinearVelocity * 0.95f + directionToPlayer * Speed * 0.5f;
        }
    }
    public void MoveContent(Vector2 offset) =>Position += offset;
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
    private void OnSceneExit()
    {
        _ifOffScreen = true;
        var timer = new Timer();
        timer.Autostart = true;
        timer.WaitTime = _OutOfScreenTimeToDie;
        timer.OneShot = true;
        AddChild(timer);
        timer.Timeout += () =>
        {
            if (_ifOffScreen)
                Death();
        };
        timer.Dispose();
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