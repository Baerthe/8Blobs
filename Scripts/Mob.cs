using Godot;
using System;

public partial class Mob : RigidBody2D
{
    [Export]
    public int Speed { get; set; }
    [Export]
    public AnimatedSprite2D Sprite2D { get; private set; }
    [Export]
    public CollisionShape2D Collision2D { get; private set; }
    [Export]
    public VisibleOnScreenNotifier2D Notifier2D { get; private set; }
	public override void _Ready()
    {
        Hide();
    }
    public override void _Process(double delta)
    {
        
    }
}