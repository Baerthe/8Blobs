namespace Equipment;
using Godot;
/// <summary>
/// A base class for all items that can be used by the player. This exists in the event I want to reuse code across multiple item types.
/// </summary>
public abstract partial class Item : Pickup
{
    public abstract void Effect(Player player);
}