namespace Entities;

using Godot;
/// <summary>
/// The Entity class for Chests, stores components and runtime data
/// </summary>
[GlobalClass]
public partial class ChestEntity : Node2D
{
    [ExportCategory("Components")]
    [ExportGroup("Components")]
    [Export] public CollisionObject2D Hitbox { get; private set; }
    [Export] public Sprite2D Sprite { get; private set; }
}