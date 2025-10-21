namespace Entities;

using Godot;
using Entities.Interfaces;
/// <summary>
/// ChestData is a Resource that defines the properties and attributes of a chest entity in the game.
/// </summary>
[GlobalClass]
public partial class ChestData : Resource, IData
{
    [ExportCategory("Stats")]
    [ExportGroup("Info")]
    [Export] public string Name { get; private set; } = "";
    [Export] public string Description { get; private set; } = "";
    [Export] public string Lore { get; private set; } = "";
    [Export] public ChestType Type { get; private set; } = ChestType.Item;
    [Export] public RarityType Rarity { get; private set; } = RarityType.Common;
    [ExportGroup("Assets")]
    [Export] public Texture2D Sprite { get; set; }
}