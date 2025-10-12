namespace Game;
using Godot;
using Game.Interface;
/// <summary>
/// The player is the main character that the user controls. This class handles movement, health, and collisions with mobs.
/// </summary>
[GlobalClass]
public sealed partial class PlayerSystem : Node2D, IGameSystem
{
	public bool IsInitialized { get; private set; } = false;
	public void Update()
    {
    }
}
