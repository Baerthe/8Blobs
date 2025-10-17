namespace Entities;

using Godot;
/// <summary>
/// The Entity class for Chests, stores components and runtime data
/// </summary>
[GlobalClass]
public partial class ChestEntity : Node2D
{
    [ExportCategory("Stats")]
    [ExportGroup("Info")]
    [Export] public string ChestName { get; private set; }
    [Export] public string Description { get; private set; }
    [ExportGroup("Components")]
    [Export] public ChestType Type { get; private set; } = ChestType.Item;
    [Export] public RarityType Rarity { get; private set; } = RarityType.Common;
    [Export] public CollisionObject2D Hitbox { get; private set; }
    [Export] public Sprite2D Sprite { get; private set; }
}