namespace Entities;
using Godot;
using System;
/// <summary>
/// WeaponEntity is a Node2D that represents a weapon in the game. It contains various properties that define the weapon's characteristics, including its name, description, lore, data, icon, and sounds. It ensures that all necessary properties are set and adds itself to the "weapons" group for easy management within the game.
/// </summary>
[GlobalClass]
public partial class WeaponEntity : Node2D
{
    [ExportCategory("Components")]
    [ExportGroup("Components")]
    [Export] public Texture2D AttackSprite { get; private set; }
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
        if (AttackSprite == null) { GD.PrintErr($"ERROR: {this.Name} does not have AttackSprite set!"); failure++; }
        if (SwingSound == null) { GD.PrintErr($"ERROR: {this.Name} does not have SwingSound set!"); failure++; }
        if (HitSound == null) { GD.PrintErr($"ERROR: {this.Name} does not have HitSound set!"); failure++; }
        if (failure > 0) throw new InvalidOperationException($"{this.Name} has failed null checking with {failure} missing components!");
    }
}