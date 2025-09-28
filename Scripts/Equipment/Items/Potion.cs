namespace Equipment;
using Godot;
/// <summary>
/// Represents a healing potion that restores health.
/// </summary>
public partial class Potion : Item
{
    public override string ItemName => "Health Potion";
    public override string ItemDescription => "A potion that restores health.";
    [Export] public override Texture2D Icon { get; set; }
    [Export] public override Sprite2D Sprite { get; set; }
    [Export] public override CollisionShape2D HitBox { get; set; }
    [Export] public override AudioStream PickUpSound { get; set; }
    [ExportGroup("Effect")]
    [Export] public int HealAmount { get; set; } = 2;
    public override void Effect(Player player)
    {
        player.Heal(HealAmount);
    }
}