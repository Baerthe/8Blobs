namespace Entities;

using Godot;
using Entities.Interfaces;
/// <summary>
/// The Data class for Heroes, stores static data
/// </summary>
[GlobalClass]
public partial class HeroData : Resource, IData
{
    [ExportCategory("Stats")]
    [ExportGroup("Info")]
    [Export] public string Name { get; private set; } = "";
    [Export] public string Description { get; private set; } = "";
    [Export] public string Lore { get; private set; } = "";
    [Export] public bool Unlocked { get; private set; } = false;
    [ExportGroup("Attributes")]
    [Export] public HeroStats Stats { get; private set; } = new HeroStats();
    [ExportGroup("Modifiers")]
    [Export] public HeroClass Class { get; private set; } = HeroClass.Warrior;
    [Export] public HeroAbility Ability { get; private set; } = HeroAbility.None;
    [Export] public uint AbilityStrength { get; private set; }
    [Export] public HeroMovement Movement { get; private set; } = HeroMovement.Walk;
    [ExportGroup("Assets")]
    [Export] public PackedScene Entity { get; set; }
    public void Unlock() => Unlocked = true;
}