namespace Entities;

using Godot;
/// <summary>
/// ChestData is a Resource that defines the properties and attributes of a chest entity in the game.
/// </summary>
[GlobalClass]
public partial class ChestData : Resource
{
    [ExportCategory("Stats")]
    [ExportGroup("Info")]
    [Export] public string ChestName { get; private set; }
    [Export] public string Description { get; private set; }
    [Export] public ChestType Type { get; private set; } = ChestType.Item;
    [Export] public RarityType Rarity { get; private set; } = RarityType.Common;
    [Export] public PackedScene ChestEntityScene { get; private set; }
}