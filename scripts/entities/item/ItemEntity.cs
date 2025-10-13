namespace Entities;
using Godot;
using System;
[GlobalClass]
public partial class ItemEntity : Node2D
{
    [ExportCategory("Stats")]
    [ExportGroup("Info")]
    [Export] public string ItemName { get; set; } = "";
    [Export] public string Description { get; set; } = "";
    [Export] public string Lore { get; set; } = "";
    [Export] public int MaxStackSize { get; set; } = 64;
    [ExportGroup("Components")]
    [Export] public ItemData Data { get; set; }
    [Export] public Texture2D Icon { get; set; }
    public int CurrentStackSize { get; set; } = 1;
    public override void _Ready()
    {
        NullCheck();
        AddToGroup("items");
    }
    private void NullCheck()
    {
        byte failure = 0;
        if (ItemName == null) { GD.PrintErr($"ERROR: {this.Name} does not have ItemName set!"); failure++; }
        if (Description == null) { GD.PrintErr($"ERROR: {this.Name} does not have Description set!"); failure++; }
        if (Lore == null) { GD.PrintErr($"ERROR: {this.Name} does not have Lore set!"); failure++; }
        if (Data == null) { GD.PrintErr($"ERROR: {this.Name} does not have Data set!"); failure++; }
        if (Icon == null) { GD.PrintErr($"ERROR: {this.Name} does not have Icon set!"); failure++; }
        if (failure > 0) throw new InvalidOperationException($"{this.Name} has failed null checking with {failure} missing components!");
    }

}