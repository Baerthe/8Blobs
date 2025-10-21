namespace Entities;

using Godot;
using Godot.Collections;
using Entities.Interfaces;
/// <summary>
/// LevelData is a Resource that encapsulates the core attributes of a game level, including its type, tier, and the mob spawn table associated with it.
/// </summary>
[GlobalClass]
public partial class LevelData : Resource, IData
{
    [ExportCategory("Stats")]
    [ExportGroup("Info")]
    [Export] public string LevelName { get; private set; } = "";
    [Export] public string Description { get; private set; } = "";
    [Export] public LevelType Type { get; set; }
    [Export] public LevelTier Tier { get; set; }
    [Export] public uint MaxLevel { get; set; } = 1;
    [Export] public float MaxTime { get; set; } = 600f;
    [ExportCategory("Scenes")]
    [Export] public Array<MobData> MobTable { get; private set; } = new();
    [Export] public PackedScene Entity { get; private set; }
}
