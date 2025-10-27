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
    [Export] public CommonInfo Info { get; private set; } = new CommonInfo();
    [Export] public CommonData MetaData { get; private set; } = new CommonData();
    [ExportGroup("Attributes")]
    [Export] public uint MaxLevel { get; set; } = 1;
    [Export] public WeaponStats Stats { get; private set; } = new WeaponStats();
    [ExportGroup("Assets")]
    [Export] public Texture2D Icon { get; private set; }
    [Export] public Texture2D AttackSprite { get; private set; }
    [Export] public AudioStream SwingSound { get; private set; }
    [Export] public AudioStream HitSound { get; private set; }
}