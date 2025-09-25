using Godot;
using System;
/// <summary>
/// The player is the main character that the user controls. This class handles movement, health, and collisions with mobs.
/// </summary>
public partial class Player : Area2D
{
	[Signal]
	public delegate void HitEventHandler();
	[Export]
	public int Health { get; set; } = 4;
	[Export]
	public int Speed { get; set; } = 400;
	[Export]
	public AnimatedSprite2D Sprite2D { get; private set; }
	[Export]
	public CollisionShape2D Collision2D { get; private set; }
	public PlayerDirection CurrentPlayerDirection { get; private set; }
	public void Start(Vector2 position)
	{
		Position = position;
		Show();
		Collision2D.Disabled = false;
	}
	public override void _Ready()
	{
		Hide();
	}
	public override void _Process(double delta)
	{
		var velocity = Vector2.Zero;
		//What direction is the player going?
		if (Input.IsActionPressed("move_up"))
		{
			velocity.Y -= 1;
			CurrentPlayerDirection = PlayerDirection.Up;
		}
		if (Input.IsActionPressed("move_down"))
		{
			velocity.Y += 1;
			CurrentPlayerDirection = PlayerDirection.Down;
		}
		if (Input.IsActionPressed("move_left"))
		{
			velocity.X -= 1;
			CurrentPlayerDirection = PlayerDirection.Left;
		}
		if (Input.IsActionPressed("move_right"))
		{
			velocity.X += 1;
			CurrentPlayerDirection = PlayerDirection.Right;
		}
		// If the player is going two directions, ie diagonal, set special state.
		if (velocity.Y != 0 && velocity.X != 0)
			CurrentPlayerDirection = PlayerDirection.Diagonal;
		if (velocity.Length() > 0)
		{
			velocity = velocity.Normalized() * Speed;
			Sprite2D.Play();
		}
		else
			Sprite2D.Stop();
		// Move the player.
		Position += velocity * (float)delta;
		Sprite2D.FlipV = false; // Make sure we never flip vertically
							  // Set animation based on direction
		switch (CurrentPlayerDirection)
		{
			case PlayerDirection.Up:
				Sprite2D.Animation = "Up";
				break;
			case PlayerDirection.Down:
				Sprite2D.Animation = "Down";
				break;
			case PlayerDirection.Diagonal:
				Sprite2D.Animation = "Right";
				Sprite2D.FlipH = velocity.X < 0;
				break;
			case PlayerDirection.Left:
				Sprite2D.FlipH = true;
				Sprite2D.Animation = "Right";
				break;
			case PlayerDirection.Right:
				Sprite2D.FlipH = false;
				Sprite2D.Animation = "Right";
				break;
			default:
				Sprite2D.Animation = "Idle";
				break;
		}
	}
	private void OnBodyEntered(Node2D body)
	{
		Health -= 1;
		EmitSignal(SignalName.Hit);
		Collision2D.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
		if (Health <= 0)
			OnDeath();
	}
	private void OnDeath()
	{
		Hide();
	}
	public enum PlayerDirection : byte
	{
		Up,
		Right,
		Down,
		Left,
		Diagonal
	}
}
