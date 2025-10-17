namespace Entities;
using Godot;
using System;
/// <summary>
/// MobEntity is a RigidBody2D that represents a mobile entity (mob) in the game. It contains various properties that define the mob's characteristics, including its name, description, lore, data, hitbox, sprite, cry sound, and visibility notifier. It ensures that all necessary properties are set and adds itself to the "mobs" group for easy management within the game.
/// </summary>
[GlobalClass]
public partial class MobEntity : RigidBody2D
{
    [ExportCategory("Stats")]
    [ExportGroup("Info")]
    [Export] public string MobName { get; private set; }
    [Export] public string Description { get; private set; }
    [Export] public string Lore { get; private set; }
    [ExportGroup("Components")]
    [Export] public MobData Data { get; private set; }
    [Export] public CollisionObject2D Hitbox { get; private set; }
    [Export] public Sprite2D Sprite { get; private set; }
    [Export] public AudioStream Cry { get; private set; }
    [Export] public VisibleOnScreenNotifier2D Notifier2D { get; private set; }
    public Vector2 CurrentVelocity { get; set; }
    public uint CurrentHealth { get; set; }
    public override void _Ready()
    {
        NullCheck();
        AddToGroup("mobs");
    }
    private void NullCheck()
    {
        byte failure = 0;
        if (MobName == null) { GD.PrintErr($"ERROR: {this.Name} does not have MobName set!"); failure++; }
        if (Description == null) { GD.PrintErr($"ERROR: {this.Name} does not have Description set!"); failure++; }
        if (Lore == null) { GD.PrintErr($"ERROR: {this.Name} does not have Lore set!"); failure++; }
        if (Data == null) { GD.PrintErr($"ERROR: {this.Name} does not have Data set!"); failure++; }
        if (Hitbox == null) { GD.PrintErr($"ERROR: {this.Name} does not have Hitbox set!"); failure++; }
        if (Sprite == null) { GD.PrintErr($"ERROR: {this.Name} does not have Sprite set!"); failure++; }
        if (Cry == null) { GD.PrintErr($"ERROR: {this.Name} does not have Cry set!"); failure++; }
        if (Notifier2D == null) { GD.PrintErr($"ERROR: {this.Name} does not have Notifier2D set!"); failure++; }
        if (failure > 0) throw new InvalidOperationException($"{this.Name} has failed null checking with {failure} missing components!");
    }
}