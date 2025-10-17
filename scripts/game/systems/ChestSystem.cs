namespace Game;
using Godot;
using Game.Interface;
public sealed partial class ChestSystem : Node2D, IGameSystem
{
    public bool IsInitialized { get; private set; } = false;
    public Path2D PickupPath { get; set; }
    public PathFollow2D PickupSpawner { get; set; }
    public Vector2 OffsetBetweenPickupAndPlayer { get; private set; }
    public override void _Ready()
    {
    }
    public void Update()
    {
    }
}