namespace Entities;
using Godot;
[GlobalClass]
public partial class MobData : Resource
{
    [ExportCategory("Stats")]
    [ExportGroup("Attributes")]
    [Export] public MobTribe Tribe { get; private set; }
    [Export] public RarityType Rarity { get; private set; } = RarityType.Basic;
    [Export] public MobLevel Level { get; private set; } = MobLevel.Basic;
    [Export] public uint Health { get; private set; } = 10;
    [Export] public uint Damage { get; private set; } = 1;
    [Export] public DamageType Attack { get; private set; } = DamageType.Blunt;
    [Export] public uint ElementDamage { get; private set; } = 0;
    [Export] public ElementType Element { get; private set; } = ElementType.None;
    [Export] public float Speed { get; private set; } = 10f;
    [Export] public byte CollisionRadius { get; private set; } = 16;
    [ExportGroup("Behavior")]
    [Export] public MobAbility Ability { get; private set; } = MobAbility.None;
    [Export] public uint AbilityStrength { get; private set; }
    [Export] public MobMovement Movement { get; private set; } = MobMovement.PlayerAttracted;
}