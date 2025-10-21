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
    [Export] public string Name { get; private set; } = "";
    [Export] public string Description { get; private set; } = "";
    [Export] public string Lore { get; private set; } = "";
    [Export] public int MaxStackSize { get; set; } = 64;
    [Export] public bool Unlocked { get; private set; } = false;
    [Export] public Texture2D Icon { get; set; }
    [ExportGroup("Scene")]
    [Export] public PackedScene Entity { get; private set; }
    public void Unlock() => Unlocked = true;
}