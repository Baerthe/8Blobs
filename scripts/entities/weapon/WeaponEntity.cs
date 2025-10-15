namespace Entities;
using Godot;
using System;
/// <summary>
/// WeaponEntity is a Node2D that represents a weapon in the game. It contains various properties that define the weapon's characteristics, including its name, description, lore, data, icon, and sounds. It ensures that all necessary properties are set and adds itself to the "weapons" group for easy management within the game.
/// </summary>
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
    [Export] public Texture2D AttackSprite { get; private set; }
    [Export] public AudioStream SwingSound { get; private set; }
    [Export] public AudioStream HitSound { get; private set; }
    public uint CurrentLevel
    {
        get
        {
            if (Data == null) return 1;
            return Math.Clamp(_currentLevel, 1, Data.MaxLevel);
        }
        set
        {
            if (Data == null) return;
            if (value < 1 || value > Data.MaxLevel)
                GD.PrintErr($"WARNING: Tried to set {this.Name}'s CurrentLevel to {value}, which is outside the valid range of 1 to {Data.MaxLevel}. Clamping to valid range.");
            _currentLevel = Math.Clamp(value, 1, Data.MaxLevel);
        }
    }
    private uint _currentLevel = 1;
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
        if (AttackSprite == null) { GD.PrintErr($"ERROR: {this.Name} does not have AttackSprite set!"); failure++; }
        if (SwingSound == null) { GD.PrintErr($"ERROR: {this.Name} does not have SwingSound set!"); failure++; }
        if (HitSound == null) { GD.PrintErr($"ERROR: {this.Name} does not have HitSound set!"); failure++; }
        if (failure > 0) throw new InvalidOperationException($"{this.Name} has failed null checking with {failure} missing components!");
    }
}