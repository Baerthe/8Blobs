namespace Mobs;

using System;
using Core;
using Godot;
/// <summary>
/// A mob is a mobile enemy that moves around the screen and can collide with the player. This is a base class for all mobs.
/// </summary>
/// <remarks>
/// Mobs have different movement types, abilities, and stats. They can take damage and die.
/// Mobs are spawned by spawners and interact with the player.
/// Mobs could be extended to have more complex behaviors and interactions; and even done completely in script, but you should use the editor for better results; making each mob into a scene.
/// </remarks>
public abstract partial class Mob : RigidBody2D
{
    [ExportCategory("Statistics")]
    [ExportSubgroup("General")]
    [Export] public MobLevel Level { get; set; } = MobLevel.Basic;
    [Export] public ElementType Element { get; set; } = ElementType.None;
    [Export] public RarityType Rarity { get; set; } = RarityType.Common;
    [Export] public MobMovement MovementType { get; private set; } = MobMovement.CurvedDirection;
    [Export] public MobAbility Ability { get; set; } = MobAbility.Runner;
    [Export] public byte AbilityStrength { get; set; } = 1;
    [ExportSubgroup("Values")]
    [Export] public byte Health { get; set; } = 1;
    [Export] public byte ExpWorth { get; set; } = 1;
    [Export] public byte Speed { get; set; } = 100;
    [Export] public byte Damage { get; set; } = 1;
    [ExportSubgroup("Info")]
    [Export] public string MobName { get; set; } = "Mob";
    [Export] public string Description { get; set; } = "A generic mob.";
    [Export] public string Lore { get; set; } = "No lore available.";
    [ExportCategory("Parts")]
    [Export] private AnimatedSprite2D _sprite2D;
    [Export] private CollisionShape2D _collision2D;
    [Export] private AudioStreamOggVorbis _audioCall;
    // Private Variables
    private bool _ifOffScreen = false;
    private Player _player = Main.GlobalPlayer;
    private bool _lock = false;
    public override void _Ready()
    {
        if (_sprite2D == null) GD.PrintErr("Mob: Sprite2D is null.");
        if (_collision2D == null) GD.PrintErr("Mob: Collision2D is null.");
        _sprite2D.Animation = "Walk";
    }
    public override void _Process(double delta)
    {
        _sprite2D.Play();
    }
    public override void _ExitTree()
    {
       Death();
    }
    /// <summary>
    /// Handles the mob's response to the game. Uses slow pulse called by group in main.
    /// </summary>
    public async void OnSlowPulseTimeout()
    {
        if (_lock) return;
        _lock = true;
        // if(!_levelRect.HasPoint(GlobalPosition)) return;
        // Need to add level bounds checking here once we have the service setup. Only process if within level bounds.
        var randomWait = (float)GD.RandRange(0.0, 0.125);
        await ToSignal(GetTree().CreateTimer(randomWait), "timeout");
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
        _lock = false;
    }
    public void OnPlayerCollision()
    {
        //
    }
    /// <summary>
    /// Handles taking damage and death of the mob.
    /// </summary>
    /// <param name="damage">The amount of damage to inflict on the mob.</param>
    public void TakeDamage(byte damage)
    {
        if (Health <= damage)
        {
            Death();
            return;
        }
        Health -= damage;
        _sprite2D.Animation = "Hurt";
        _sprite2D.Play();
    }
    /// <summary>
    /// Heals the mob by a specified amount, capping at 255.
    /// </summary>
    /// <param name="amount">The amount of health to restore.</param>
    public void Heal(byte amount)
    {
        Health += amount;
        if(Health > 255) Health = 255;
    }
    /// <summary>
    /// Spawns the mob at the spawner's position and sets its initial velocity based on its movement type.
    /// </summary>
    /// <param name="playerPosition"></param>
    /// <param name="spawner"></param>
    public void Spawn(Vector2 playerPosition, PathFollow2D spawner)
    {
        Position = spawner.GlobalPosition;
        float mobSpeedModifier = Speed + (int)(GD.RandRange(-1.0, 0.5) % Speed);
        float randomAngle = (float)GD.RandRange(-0.2, 0.2);
        // If the mob is attracted to the player, calculate direction towards player with some randomness
        if (MovementType == MobMovement.PlayerAttracted)
        {
            Vector2 directionToPlayer = (playerPosition - spawner.GlobalPosition).Normalized();
            directionToPlayer = directionToPlayer.Rotated(randomAngle);
            LinearVelocity = directionToPlayer * mobSpeedModifier;
            return;
        }
        float direction = spawner.Rotation + Mathf.Pi / 2;
        // If the mob moves in a random direction, add some randomness to the direction
        if (MovementType == MobMovement.RandomDirection)
        {
            direction += randomAngle;
        }
        // If the mob moves in a curved direction, calculate velocity with some randomness
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
    public enum MobLevel : byte
    {
        Basic = 1,
        Advanced = 2,
        Elite = 3,
        Boss = 4
    }
    public enum MobAbility : byte
    {
        Runner = 0,
        Poison = 1,
        Healer = 2,
        Explodes = 3,
        Aura = 4
    }
    private async void Death()
    {
        _sprite2D.Animation = "Death";
        _collision2D.Disabled = true;
        await ToSignal(_sprite2D, "animation_finished");
        QueueFree();
    }
}