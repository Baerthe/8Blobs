namespace Entities;

using Godot;
public sealed partial class EntityIndex : Resource
{
    [Export] public PackedScene ChestTemplate { get; private set; }
    [Export] public PackedScene ItemTemplate { get; private set; }
    [Export] public PackedScene MobTemplate { get; private set; }
    [Export] public PackedScene WeaponTemplate { get; private set; }
}