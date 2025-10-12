namespace Entities;

using Godot;
[GlobalClass]
public partial class MobInfo : Resource
{
    [ExportCategory("Stats")]
    [ExportGroup("Info")]
    [Export] public string MobName { get; private set; } = "Mob";
    [Export] public string Description { get; private set; } = "A generic mob.";
    [Export] public string Lore { get; private set; } = "No lore available.";
    [Export] public MobTribe Tribe { get; private set; } = MobTribe.None;
}