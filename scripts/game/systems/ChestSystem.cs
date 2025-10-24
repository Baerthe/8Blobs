namespace Game;

using Godot;
using Entities;
using Game.Interface;
using Core;
/// <summary>
/// ChestSystem is responsible for managing chest spawning and interactions within the game. It implements the IGameSystem interface and utilizes a Path2D and PathFollow2D to determine chest spawn locations relative to the player.
/// </summary>
public sealed partial class ChestSystem : Node2D, IGameSystem
{
    public bool IsInitialized { get; private set; } = false;
    public Path2D ChestPath { get; private set; }
    public PathFollow2D ChestSpawner { get; private set; }
    public Vector2 OffsetBetweenChestAndPlayer { get; private set; }
    public HeroEntity PlayerInstance { get; set; }
    public override void _Ready()
    {
        GD.Print("ChestSystem Present.");
        GetParent<GameManager>().OnLevelLoad += (sender, args) => OnLevelLoad(args.Templates, args.LevelInstance, args.PlayerInstance);
    }
    public void OnLevelLoad(EntityIndex _, LevelEntity levelInstance, HeroEntity playerInstance)
    {
        if (IsInitialized) return;
        PlayerInstance = playerInstance;
        CoreProvider.ClockService().ChestSpawnTimeout += OnChestSpawnTimeout;
        ChestPath = CreatePath();
        ChestSpawner = new PathFollow2D();
        ChestPath.AddChild(ChestSpawner);
        PlayerInstance.AddChild(ChestPath);
        IsInitialized = true;
    }
    /// <summary>
    /// Updates the offset between the chest and the player instance.
    /// </summary>
    public void Update()
    {
        OffsetBetweenChestAndPlayer = PlayerInstance.Position - ChestPath.Position;
    }
    /// <summary>
    /// Creates a circular Path2D for chest spawning.
    /// </summary>
    private Path2D CreatePath()
    {
        var path = new Path2D();
        path.Curve = new Curve2D();
        path.Curve.AddPoint(new Vector2(0, 0));
        for (int i = 1; i <= 10; i++)
        {
            var angle = i * (Mathf.Pi * 2 / 10);
            var radius = 48;
            var x = radius * Mathf.Cos(angle);
            var y = radius * Mathf.Sin(angle);
            path.Curve.AddPoint(new Vector2(x, y));
        }
        return path;
    }
    /// <summary>
    /// Handles the ChestSpawnTimeout event to spawn a new chest.
    /// </summary>
    private void OnChestSpawnTimeout()
    {
        if (!IsInitialized) return;
        ChestSpawner.ProgressRatio = GD.Randf();
    }
}