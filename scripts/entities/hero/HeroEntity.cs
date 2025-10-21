namespace Entities;

using Godot;
using System;
using Entities.Interfaces;
/// <summary>
/// The Entity class for Heroes, stores components and runtime data
/// </summary>
[GlobalClass]
public partial class HeroEntity : CharacterBody2D, IEntity
{
    [ExportCategory("Stats")]
    [ExportGroup("Components")]
    [Export] public CollisionObject2D Hitbox { get; private set; }
    [Export] public AnimatedSprite2D Sprite { get; private set; }
    [Export] public AudioStream Cry { get; private set; }
    public Vector2 CurrentVelocity { get; set; }
    public uint CurrentHealth { get; set; }
    public PlayerDirection CurrentDirection { get; set; }
    public IData Data { get; private set; }
    public override void _Ready()
    {
        NullCheck();
        AddToGroup("players");
    }
    public void InitializeEntity(IData data)
    {
        if (Data != null)
        {
            GD.PrintErr($"HeroEntity {Name} already initialized with data!");
            return;
        }
        Data = data ?? throw new ArgumentNullException(nameof(data));
    }
    public void NullCheck()
    {
        byte failure = 0;
        if (Hitbox == null) { GD.PrintErr($"ERROR: {this.Name} does not have Hitbox set!"); failure++; }
        if (Sprite == null) { GD.PrintErr($"ERROR: {this.Name} does not have Sprite set!"); failure++; }
        if (Cry == null) { GD.PrintErr($"ERROR: {this.Name} does not have Cry set!"); failure++; }
        if (failure > 0) throw new InvalidOperationException($"{this.Name} has failed null checking with {failure} missing components!");
    }
}