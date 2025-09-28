namespace Equipment;

using Godot;
/// <summary>
/// A base class for all items that can be used by the player.
///  </summary>
public abstract partial class Pickup : Node2D
{
    public abstract string ItemName { get; }
    public abstract string ItemDescription { get; }
    public abstract Texture2D Icon { get; set; }
    public abstract Sprite2D Sprite { get; set; }
    public abstract CollisionShape2D HitBox { get; set; }
    public abstract AudioStream PickUpSound { get; set; }
}