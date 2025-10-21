namespace Entities;

using Godot;
[GlobalClass]
public partial class WeaponStats : Resource
{
    [Export] public uint Damage { get; set; }
    [Export] public DamageType DamageType { get; set; } = DamageType.Blunt;
    [Export] public uint ElementDamage { get; set; } = 0;
    [Export] public ElementType Element { get; set; } = ElementType.None;
    [Export] public float AttackSpeed { get; set; }
    [Export] public float Range { get; set; }
}