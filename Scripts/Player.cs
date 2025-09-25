using Godot;
using System;

public partial class Player : Area2D
{
	[Signal]
	/// <summary>
	/// Delegate for when the player is hit by an enemy.
	/// </summary>
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
	private Vector2 _screenSize;
	public void Start(Vector2 position)
	{
		Position = position;
		Show();
		Collision2D.Disabled = false;
	}
	public override void _Ready()
	{
		Hide();
		_screenSize = GetViewportRect().Size;
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
		Position = new Vector2(x: Mathf.Clamp(Position.X, 0, _screenSize.X), y: Mathf.Clamp(Position.Y, 0, _screenSize.Y));
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
