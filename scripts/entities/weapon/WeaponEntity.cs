namespace Entities;
using Godot;
using System;
[GlobalClass]
public partial class WeaponEntity : Node2D
{
    [ExportCategory("Stats")]
    [ExportGroup("Info")]
    [Export] public string WeaponName { get; private set; }
    [Export] public string Description { get; private set; }
    [Export] public string Lore { get; private set; }
    [ExportGroup("Components")]
    [Export] public WeaponData Data { get; private set; }
    [Export] public Texture2D Icon { get; private set; }
    [Export] public AudioStream SwingSound { get; private set; }
    [Export] public AudioStream HitSound { get; private set; }
    public override void _Ready()
    {
        NullCheck();
        AddToGroup("weapons");
    }
    private void NullCheck()
    {
        byte failure = 0;
        if (WeaponName == null) { GD.PrintErr($"ERROR: {this.Name} does not have WeaponName set!"); failure++; }
        if (Description == null) { GD.PrintErr($"ERROR: {this.Name} does not have Description set!"); failure++; }
        if (Lore == null) { GD.PrintErr($"ERROR: {this.Name} does not have Lore set!"); failure++; }
        if (Data == null) { GD.PrintErr($"ERROR: {this.Name} does not have Data set!"); failure++; }
        if (Icon == null) { GD.PrintErr($"ERROR: {this.Name} does not have Icon set!"); failure++; }
        if (SwingSound == null) { GD.PrintErr($"ERROR: {this.Name} does not have SwingSound set!"); failure++; }
        if (HitSound == null) { GD.PrintErr($"ERROR: {this.Name} does not have HitSound set!"); failure++; }
        if (failure > 0) throw new InvalidOperationException($"{this.Name} has failed null checking with {failure} missing components!");
    }
}