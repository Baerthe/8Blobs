namespace Entities;

using Godot;
using Entities.Interfaces;
/// <summary>
/// Data container for mobs
/// </summary>
[GlobalClass]
public partial class MobData : Resource, IData
{
    [ExportCategory("Stats")]
    [ExportGroup("Info")]
    [Export] public string Name { get; private set; } = "";
    [Export] public string Description { get; private set; } = "";
    [Export] public string Lore { get; private set; } = "";
    [ExportGroup("Attributes")]
    [Export] public MobTribe Tribe { get; private set; }
    [Export] public RarityType Rarity { get; private set; } = RarityType.Basic;
    [Export] public MobLevel Level { get; private set; } = MobLevel.Basic;
    [Export] public MobStats Stats { get; private set; } = new MobStats();
    [ExportGroup("Behavior")]
    [Export] public MobAbility Ability { get; private set; } = MobAbility.None;
    [Export] public uint AbilityStrength { get; private set; }
    [Export] public MobMovement Movement { get; private set; } = MobMovement.PlayerAttracted;
    [ExportGroup("Assets")]
    [Export] public SpriteFrames Sprite { get; set; }
    [Export] public AudioStream HitSound { get; set; }
    [Export] public AudioStream DeathSound { get; set; }
    [Export] public Color TintColor { get; set; } = Colors.White;
    [Export] public Shape2D CollisionShape { get; set; }
    private static readonly Shader _defaultAnimationShader;
    [Export] public Shader AnimationShader { get; set; } = _defaultAnimationShader;
    static MobData()
    {
        _defaultAnimationShader = ResourceLoader.Load<Shader>("res://data/shaders/mobs/BasicMobMovement.gdshader");
    }
}