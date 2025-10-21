namespace Entities;

using Godot;
using Entities.Interfaces;
/// <summary>
/// WeaponData is a Resource that defines the properties and attributes of a weapon entity in the game.
/// </summary>
[GlobalClass]
public partial class WeaponData : Resource, IData
{
    [ExportCategory("Stats")]
    [ExportGroup("Info")]
    [Export] public string WeaponName { get; private set; }
    [Export] public string Description { get; private set; }
    [Export] public string Lore { get; private set; }
    [ExportGroup("Attributes")]
    [Export] public Texture2D Icon { get; private set; }
    [Export] public RarityType Rarity { get; set; } = RarityType.Basic;
    [Export] public uint MaxLevel { get; set; } = 1;
    [Export] public uint Damage { get; set; }
    [Export] public DamageType DamageType { get; set; } = DamageType.Blunt;
    [Export] public uint ElementDamage { get; set; } = 0;
    [Export] public ElementType Element { get; set; } = ElementType.None;
    [Export] public float AttackSpeed { get; set; }
    [Export] public float Range { get; set; }
    [ExportGroup("Scene")]
    [Export] public PackedScene Entity { get; private set; }
}