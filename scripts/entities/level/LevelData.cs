namespace Entities;
using Godot;
[GlobalClass]
public partial class LevelData : Resource
{
    [Export] public LevelType Type { get; set; }
    [Export] public LevelTier Tier { get; set; }
    [Export] public MobIndex SpawnTable { get; private set; }
}
