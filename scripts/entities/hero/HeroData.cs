namespace Entities;
using Godot;
using System;
[GlobalClass]
public partial class HeroData : Resource
{
    [ExportCategory("Stats")]
    [ExportGroup("Attributes")]
    [Export] public HeroLevel Level { get; private set; } = HeroLevel.Basic;
    [Export] public uint Health { get; private set; } = 100;
    [Export] public uint DamageBonus { get; private set; } = 0;
    [Export] public uint ElementDamageBonus { get; private set; } = 0;
    [Export] public ElementType ElementBonus { get; private set; } = ElementType.None;
    [Export] public float Speed { get; private set; } = 100f;
    [Export] public byte CollisionRadius { get; private set; } = 16;
    [ExportGroup("Modifiers")]
    [Export] public HeroClass Class { get; private set; } = HeroClass.Warrior;
    [Export] public HeroAbility Ability { get; private set; } = HeroAbility.None;
    [Export] public uint AbilityStrength { get; private set; }
    [Export] public HeroMovement Movement { get; private set; } = HeroMovement.Walk;
}