namespace Equipment;

using System;
using Godot;
/// <summary>
/// A base class for all weapons that can be used by the player.
/// </summary>
public abstract partial class Weapon : Pickup
{
    abstract public int Damage { get; set; }
    abstract public float AttackSpeed { get; set; } // Attacks per second
    abstract public float Range { get; set; } // In pixels
    abstract public Sprite2D ProjectileSprite { get; set; }
    abstract public AudioStream AttackSound { get; set; }
    abstract public void Attack(Double deltaTime);
}