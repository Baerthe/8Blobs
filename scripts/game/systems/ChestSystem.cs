namespace Game;
using Godot;
using Game.Interface;
public sealed partial class ChestSystem : Node2D, IGameSystem
{
    public bool IsInitialized { get; private set; } = false;
    public override void _Ready()
    {
    }
    public void Update()
    {
    }
}