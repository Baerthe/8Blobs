using Godot;
using System;

public partial class Player : Area2D
{
	[Export]
	public int Speed { get; set; } = 400;
	[Export]
	public AnimatedSprite2D sprite { get; private set; }
	private Vector2 _screenSize;
	private PlayerDirection _playerDirection;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_screenSize = GetViewportRect().Size;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var velocity = Vector2.Zero;
		if (Input.IsActionPressed("move_up")) velocity.Y -= 1;
		if (Input.IsActionPressed("move_down")) velocity.Y += 1;
		if (Input.IsActionPressed("move_left")) velocity.X -= 1;
		if (Input.IsActionPressed("move_right")) velocity.X += 1;
		if (velocity.Length() > 0)
		{
			velocity = velocity.Normalized() * Speed;
			sprite.Play();
		}
		else
		{
			sprite.Stop();
		}
		Position += velocity * (float)delta;
		Position = new Vector2(x: Mathf.Clamp(Position.X, 0, _screenSize.X), y: Mathf.Clamp(Position.Y, 0, _screenSize.Y));
		sprite.FlipV = false; // Make sure we never flip vertically
		// Which direction is the player going?
		if (velocity.Y != 0 && velocity.X != 0)
			_playerDirection = PlayerDirection.Diagonal;
		else if (velocity.Y != 0)
			if (velocity.Normalized().Y == 1)
				_playerDirection = PlayerDirection.Down;
			else
				_playerDirection = PlayerDirection.Up;
		else if (velocity.X != 0)
			if (sprite.FlipH = velocity.X < 0)
				_playerDirection = PlayerDirection.Left;
			else
				_playerDirection = PlayerDirection.Right;
		// Set animation based on direction
		switch (_playerDirection)
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
				sprite.Animation = "Right";
				sprite.FlipH = true;
				break;
			case PlayerDirection.Right:
				sprite.Animation = "Right";
				break;
			default:
				sprite.Animation = "Idle";
				break;
		}
	}
	private enum PlayerDirection : byte
	{
		Up,
		Right,
		Down,
		Left,
		Diagonal
	}
}
