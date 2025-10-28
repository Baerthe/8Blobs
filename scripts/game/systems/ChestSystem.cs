namespace Game;

using Godot;
using Core;
using Entities;
using Game.Interface;
using Core.Interface;
/// <summary>
/// ChestSystem is responsible for managing chest spawning and interactions within the game. It implements the IGameSystem interface and utilizes a Path2D and PathFollow2D to determine chest spawn locations relative to the player.
/// </summary>
public sealed partial class ChestSystem : Node2D, IGameSystem
{
    public bool IsInitialized { get; private set; } = false;
    private HeroEntity _playerRef;
    private Path2D _chestPath;
    private PathFollow2D _chestSpawner;
    private Vector2 _offsetBetweenChestAndPlayer;
    private PackedScene _chestTemplate;
    // Dependency Services
    private readonly IAudioService _audioService;
    private readonly IEventService _eventService;
    public ChestSystem(PackedScene chestTemplate)
    {
        GD.Print("ChestSystem: Initializing...");
        _chestTemplate = chestTemplate;
        _audioService = CoreProvider.AudioService();
        _eventService = CoreProvider.EventService();
    }
    public override void _Ready()
    {
        _eventService.Subscribe<Init>(OnInit);
        _eventService.Subscribe<ChestSpawnTimeout>(OnChestSpawnTimeout);
        GD.Print("ChestSystem Ready.");
    }
    public override void _ExitTree()
    {
        _eventService.Unsubscribe<Init>(OnInit);
        _eventService.Unsubscribe<ChestSpawnTimeout>(OnChestSpawnTimeout);
    }
    public void OnInit()
    {
        if (IsInitialized) return;
        _playerRef = GetTree().GetFirstNodeInGroup("Player") as HeroEntity;
        _chestPath = CreatePath();
        _chestSpawner = new PathFollow2D();
        _chestPath.AddChild(_chestSpawner);
        _playerRef.AddChild(_chestPath);
        IsInitialized = true;
    }
    /// <summary>
    /// Updates the offset between the chest and the player instance.
    /// </summary>
    public void OnPulseTimeOut()
    {
        _offsetBetweenChestAndPlayer = _playerRef.Position - _chestPath.Position;
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
        _chestSpawner.ProgressRatio = GD.Randf();
        // TODO: Add logic to check for existing chests and limit the number of active chests.
    }
}