namespace Entities;

using System;
using Godot;
public partial class MobEntity : RigidBody2D
{
    [Export] public MobInfo Info { get; private set; }
    [Export] public MobData Data { get; private set; }
    [Export] public CollisionObject2D Hitbox { get; private set; }
    [Export] public Sprite2D Sprite { get; private set; }
    [Export] public AudioStreamOggVorbis Cry { get; private set; }
    public Vector2 Velocity { get; set; }
    public uint CurrentHealth { get; set; }
    public VisibleOnScreenEnabler2D Enabler { get; private set; } = new();
    public override void _Ready()
    {
        AddChild(Enabler);
        AddToGroup("mobs");
    }
}