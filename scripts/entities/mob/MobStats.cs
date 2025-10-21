namespace Entities;

using Godot;
/// <summary>
/// Data container for mob stats
/// </summary>
[GlobalClass]
public partial class MobStats : Resource
{
    [Export] public uint Health { get; set; } = 10;
    [Export] public uint Damage { get; set; } = 1;
    [Export] public float Speed { get; set; } = 100f;
    [Export] public DamageType AttackType { get; set; } = DamageType.Blunt;
    [Export] public ElementType Element { get; set; } = ElementType.None;
}