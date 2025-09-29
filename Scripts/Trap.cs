namespace Mobs;
using Godot;
public partial class Trap : RigidBody2D
{
	[Export] public int Damage = 1;
	[Export] private CollisionShape2D _collision2D;
    public void MoveContent(Vector2 offset) =>Position += offset;
}
