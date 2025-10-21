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
    [Export] public string Name { get; private set; } = "";
    [Export] public string Description { get; private set; } = "";
    [Export] public string Lore { get; private set; } = "";
    [ExportGroup("Attributes")]
    [Export] public RarityType Rarity { get; set; } = RarityType.Basic;
    [Export] public uint MaxLevel { get; set; } = 1;
    [Export] public WeaponStats Stats { get; private set; } = new WeaponStats();
    [Export] public bool Unlocked { get; private set; } = false;
    [ExportGroup("Assets")]
    [Export] public Texture2D Icon { get; private set; }
    [Export] public Texture2D AttackSprite { get; private set; }
    [Export] public AudioStream SwingSound { get; private set; }
    [Export] public AudioStream HitSound { get; private set; }
    public void Unlock() => Unlocked = true;
}