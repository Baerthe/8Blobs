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
    [Export] public CommonInfo Info { get; private set; } = new CommonInfo();
    [Export] public CommonData MetaData { get; private set; } = new CommonData();
    [Export] public ChestType Type { get; private set; } = ChestType.Item;
    [ExportGroup("Assets")]
    [Export] public SpriteFrames Sprite { get; set; }
}