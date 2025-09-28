namespace Equipment;
using Godot;
/// <summary>
/// Represents a healing potion that restores health.
/// </summary>
public partial class Potion : Item
{
    [Export] public override string ItemName { get; set; } = "Health Potion";
    [Export] public override string ItemDescription { get; set; } = "A potion that restores health.";
    [Export] public override Texture2D Icon { get; set; }
    [Export] public override Sprite2D Sprite { get; set; }
    [Export] public override CollisionShape2D HitBox { get; set; }
    [Export] public override AudioStreamPlayer2D PickUpSound { get; set; }
    [ExportGroup("Effect")]
    [Export] public int HealAmount { get; private set; } = 2;
    public override void Effect(Player player)
    {
        player.Heal(HealAmount);
    }
}