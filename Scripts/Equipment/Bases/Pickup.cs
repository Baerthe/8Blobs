namespace Equipment;
using Godot;
/// <summary>
/// A base class for all pickups that can be used by the player.
///  </summary>
public abstract partial class Pickup : RigidBody2D
{
    public abstract string PickupName { get; set; }
    public abstract string PickupDescription { get; set; }
    public abstract Texture2D Icon { get; set; }
    public abstract Sprite2D Sprite { get; set; }
    public abstract CollisionShape2D HitBox { get; set; }
    public abstract AudioStreamPlayer2D PickUpSound { get; set; }
    public void PickupEquipment()
    {
        PickUpSound.Play();
    }
    public void MoveContent(Vector2 offset) =>Position += offset;
}