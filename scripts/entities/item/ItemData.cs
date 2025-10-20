namespace Entities;

using Godot;
/// <summary>
/// ItemData is a Resource that defines the properties and attributes of an item entity in the game.
/// </summary>
[GlobalClass]
public partial class ItemData : Resource
{
    [ExportCategory("Stats")]
    [ExportGroup("Info")]
    [Export] public string ItemName { get; set; } = "";
    [Export] public string Description { get; set; } = "";
    [Export] public string Lore { get; set; } = "";
    [Export] public int MaxStackSize { get; set; } = 64;
    [Export] public Texture2D Icon { get; set; }
    [ExportGroup("Scene")]
    [Export] public PackedScene ItemEntityScene { get; private set; }
}