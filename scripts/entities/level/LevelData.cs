namespace Entities;

using Godot;
using Godot.Collections;
/// <summary>
/// LevelData is a Resource that encapsulates the core attributes of a game level, including its type, tier, and the mob spawn table associated with it.
/// </summary>
[GlobalClass]
public partial class LevelData : Resource
{
    [Export] public LevelType Type { get; set; }
    [Export] public LevelTier Tier { get; set; }
    [Export] public Array<PackedScene> MobTable { get; private set; } = new();
}
