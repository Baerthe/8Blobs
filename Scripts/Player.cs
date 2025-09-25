using Godot;
using System;

public partial class Player : Area2D
{
	[Signal]
	public delegate void HitEventHandler();
	[Export]
	public int Speed { get; set; } = 400;
	[Export]
	public AnimatedSprite2D sprite { get; private set; }
	public PlayerDirection playerDirection { get; private set; }
	private Vector2 _screenSize;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//Hide();
		_screenSize = GetViewportRect().Size;
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var velocity = Vector2.Zero;
		//What direction is the player going?
		if (Input.IsActionPressed("move_up"))
		{
			velocity.Y -= 1;
			playerDirection = PlayerDirection.Up;
		}
		if (Input.IsActionPressed("move_down"))
		{
			velocity.Y += 1;
			playerDirection = PlayerDirection.Down;
		}
		if (Input.IsActionPressed("move_left"))
		{
			velocity.X -= 1;
			playerDirection = PlayerDirection.Left;
		}
		if (Input.IsActionPressed("move_right"))
		{
			velocity.X += 1;
			playerDirection = PlayerDirection.Right;
		}
		// If the player is going two directions, ie diagonal, set special state.
		if (velocity.Y != 0 && velocity.X != 0)
			playerDirection = PlayerDirection.Diagonal;
		if (velocity.Length() > 0)
		{
			velocity = velocity.Normalized() * Speed;
			sprite.Play();
		}
		else
			sprite.Stop();
		// Move the player.
		Position += velocity * (float)delta;
		Position = new Vector2(x: Mathf.Clamp(Position.X, 0, _screenSize.X), y: Mathf.Clamp(Position.Y, 0, _screenSize.Y));
		sprite.FlipV = false; // Make sure we never flip vertically
		// Set animation based on direction
		switch (playerDirection)
		{
			case PlayerDirection.Up:
				sprite.Animation = "Up";
				break;
			case PlayerDirection.Down:
				sprite.Animation = "Down";
				break;
			case PlayerDirection.Diagonal:
				sprite.Animation = "Right";
				sprite.FlipH = velocity.X < 0;
				break;
			case PlayerDirection.Left:
				sprite.FlipH = true;
				sprite.Animation = "Right";
				break;
			case PlayerDirection.Right:
				sprite.FlipH = false;
				sprite.Animation = "Right";
				break;
			default:
				sprite.Animation = "Idle";
				break;
		}
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
