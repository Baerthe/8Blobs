namespace Entities;

using Godot;
using Entities.Interfaces;
/// <summary>
/// ItemData is a Resource that defines the properties and attributes of an item entity in the game.
/// </summary>
[GlobalClass]
public partial class ItemData : Resource, IData
{
    [ExportCategory("Stats")]
    [ExportGroup("Info")]
    [Export] public CommonInfo Info { get; private set; } = new CommonInfo();
    [Export] public CommonData MetaData { get; private set; } = new CommonData();
    [Export] public int MaxStackSize { get; set; } = 64;
    [ExportGroup("Assets")]
    [Export] public SpriteFrames Sprite { get; set; }
}