namespace Equipment;
using Godot;
/// <summary>
/// A pickup in the game world that represents an item or weapon the player can collect.
/// </summary>
public partial class Equipment : Area2D
{
    [Export] private Pickup PickupEquipment { get; set; }

    public override void _Ready()
    {
        BodyEntered += OnPlayerPickup;
    }

    private void OnPlayerPickup(Node2D body)
    {
        if (body is Player player)
        {
            //player.CollectItem(PickupEquipment);
            QueueFree(); // Remove from world
        }
    }
}