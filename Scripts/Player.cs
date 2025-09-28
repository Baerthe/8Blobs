using Godot;
using Equipment;
using Mobs;
using System.Collections.Generic;
/// <summary>
/// The player is the main character that the user controls. This class handles movement, health, and collisions with mobs.
/// </summary>
public partial class Player : CharacterBody2D
{
	[Signal] public delegate void OnHitEventHandler();
	[Signal] public delegate void OnDeathEventHandler();
	[Export] public int MaxHealth { get; set; } = 4;
	public int Health { get { return _health; } }
	[Export] public int Speed { get; set; } = 400;
	[Export] private AnimatedSprite2D _sprite2D;
	[Export] private CollisionShape2D _hitBox2D;
	public PlayerDirection CurrentPlayerDirection { get; private set; }
	public List<Weapon> Weapons { get; } = new List<Weapon>();
	//public Item[] Items { get; } = new Item[2];
	private double _deltaTime = 0.1;
	private int _health = 0;
	public void Start(Vector2 position)
	{
		_health = MaxHealth;
		Position = position;
		_hitBox2D.Disabled = false;
		Show();
		SetPhysicsProcess(true);
	}
	public override void _Ready()
	{
		SetPhysicsProcess(false);
		Hide();
	}
	public override void _Process(double delta)
	{
		if (_health <= 0)
				Death();
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
		ExecuteWeaponAttacks();
	}
	// Public Support Functions for Effects
	public void Heal(int amount)
	{
		_health += amount;
		if (_health > MaxHealth)
			_health = MaxHealth;
	}
	public void Kill()
	{
		_health = 0;
		Death();
	}
	// Private Support Functions
	private void ExecuteWeaponAttacks()
	{
		for (int i = 0; i < Weapons.Count; i++)
			Weapons[i].Attack(_deltaTime);
	}
	private void OnBodyEntered(Node2D body)
	{
		GD.Print($"Body entered: {body.Name} - Type: {body.GetType().Name}");
		if (body is Pickup pickup)
		{
			GD.Print("Picked up " + pickup.PickupName);
			OnPickUp(pickup);
			return;
		}
		if (body is Mob mob)
		{
			_health -= 1;
			_hitBox2D.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
			ImmunityFrames();
		}
		EmitSignal(SignalName.OnHit);
	}
	private void OnPickUp(Pickup pickup)
	{
		GD.Print("Collecting " + pickup.PickupName);
		if (pickup is Item item)
		{
			item.Effect(this);
			pickup.QueueFree();
			// // Collect item
			// for (int i = 0; i < Items.Length; i++)
			// {
			// 	if (Items[i] == null)
			// 	{
			// 		Items[i] = item;
			// 		item.Effect(this);
			// 		pickup.QueueFree();
			// 		return;
			// 	}
			// }
		}
		else if (pickup is Weapon weapon)
		{
			// Collect weapon
			if (!Weapons.Contains(weapon))
			{
				Weapons.Add(weapon);
				pickup.QueueFree();
			}
		}
	}
	private void Death()
	{
		_sprite2D.Animation = "Death";
		// for (int i = 0; i < Items.Length; i++)
		// 	Items[i] = null;
		Weapons.Clear();
		EmitSignal(SignalName.OnDeath);
	}
	private async void ImmunityFrames()
	{
		var flashTimer = new Timer();
		flashTimer.WaitTime = _deltaTime * 5;
		flashTimer.OneShot = false;
		AddChild(flashTimer);
		flashTimer.Start();
		_sprite2D.Modulate = new Color(1, 0.5f, 0.5f); // Tint red on hit
		for (int i = 0; i < 10; i++)
		{
			_sprite2D.Visible = !_sprite2D.Visible;
			await ToSignal(flashTimer, Timer.SignalName.Timeout);
		}
		_sprite2D.Visible = true;
		_hitBox2D.Disabled = false;
		_sprite2D.Modulate = Colors.White;
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
