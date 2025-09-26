using Godot;
using System;
/// <summary>
/// The player is the main character that the user controls. This class handles movement, health, and collisions with mobs.
/// </summary>
public partial class Player : CharacterBody2D
{
	[Signal] public delegate void OnHitEventHandler();
	[Signal] public delegate void OnDeathEventHandler();
	[Export] public int Health { get; set; } = 4;
	[Export] public int Speed { get; set; } = 400;
	[Export] private AnimatedSprite2D _sprite2D;
	[Export] private CollisionShape2D _hitBox2D;
	public PlayerDirection CurrentPlayerDirection { get; private set; }
	private double _deltaTime = 0.1;
	public void Start(Vector2 position)
	{
		Position = position;
		Show();
		_hitBox2D.Disabled = false;
	}
	public override void _Ready()
	{
		Hide();
	}
	public override void _Process(double delta)
	{
		_deltaTime = delta;
		// Set animation based on direction
		switch (CurrentPlayerDirection)
		{
			case PlayerDirection.Up:
				_sprite2D.Animation = "Up";
				break;
			case PlayerDirection.Down:
				_sprite2D.Animation = "Down";
				break;
			case PlayerDirection.Diagonal:
				_sprite2D.Animation = "Right";
				break;
			case PlayerDirection.Left:
				_sprite2D.FlipH = true;
				_sprite2D.Animation = "Right";
				break;
			case PlayerDirection.Right:
				_sprite2D.FlipH = false;
				_sprite2D.Animation = "Right";
				break;
			default:
				_sprite2D.Animation = "Idle";
				break;
		}
	}
	public override void _PhysicsProcess(double delta)
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
			_sprite2D.Play();
		}
		else
			_sprite2D.Stop();
		// Move the player.
		Position += velocity * (float)delta;
		_sprite2D.FlipV = false; // Make sure we never flip vertically
		_sprite2D.FlipH = velocity.X < 0;
		MoveAndSlide();
	}
	private void OnBodyEntered(RigidBody2D body)
	{
		GD.Print("Player hit by mob");
		Health -= 1;
		EmitSignal(SignalName.OnHit);
		_hitBox2D.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
		ImmunityFrames();
		if (Health <= 0)
			Death();
	}
	private async void Death()
	{
		_sprite2D.Animation = "Death";
		var deathTimer = new Timer();
		deathTimer.WaitTime = 1.0;
		deathTimer.OneShot = true;
		AddChild(deathTimer);
		deathTimer.Start();
		await ToSignal(deathTimer, Timer.SignalName.Timeout);
		Hide();
		deathTimer.QueueFree();
		EmitSignal(SignalName.OnDeath);
	}
	private async void ImmunityFrames()
	{
		var flashTimer = new Timer();
		flashTimer.WaitTime = _deltaTime * 5;
		flashTimer.OneShot = false;
		AddChild(flashTimer);
		flashTimer.Start();
		for (int i = 0; i < 10; i++)
		{
			_sprite2D.Visible = !_sprite2D.Visible;
			await ToSignal(flashTimer, Timer.SignalName.Timeout);
		}
		_sprite2D.Visible = true;
		_hitBox2D.Disabled = false;
		flashTimer.QueueFree();
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
