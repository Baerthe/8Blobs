namespace Game;

using Godot;
using Core;
using Entities;
using Game.Interface;
using Core.Interface;
using System.Collections.Generic;
using System;

/// <summary>
/// MobSystem is responsible for managing all mobile entities (mobs) within the game.
/// It handles their initialization, spawning, AI behavior, and updates during the game loop.
/// The system maintains a pool of mobs based on the current level's data and processes their actions based on predefined AI movement types.
/// </summary>
public sealed partial class MobSystem : Node2D, IGameSystem
{
    public bool IsInitialized { get; private set; } = false;
    private List<(MobData mob, float weight)> _mobTable;
    private Dictionary<MobMovement, System.Action<MobEntity>> _aiHandlers;
    private List<MobEntity> _pooledMobs = new();
    private List<MobEntity> _activeMobs = new();
    private HeroEntity _playerRef = null;
    private LevelEntity _levelRef = null;
    private Path2D[] _mobSpawnPaths;
    private PathFollow2D[] _mobSpawners;
    private PackedScene _mobTemplate;
    private float _mobWeights = 0f;
    private float _gameElapsedTime = 0f;
    private double _deltaTime = 0.0;
    private RandomNumberGenerator _random = new RandomNumberGenerator();
    // Dependency Services
    private readonly IAudioService _audioService;
    private readonly IEventService _eventService;
    public MobSystem(PackedScene mobTemplate, IAudioService audioService, IEventService eventService)
    {
        GD.Print("MobSystem: Initializing...");
        _audioService = audioService;
        _eventService = eventService;
        _mobTemplate = mobTemplate;
    }
    public override void _Ready()
    {
        _eventService.Subscribe<Init>(OnInit);
        _eventService.Subscribe<MobSpawnTimeout>(OnMobTimeout);
        _eventService.Subscribe<GameTimeout>(OnGameTimeout);
        GD.Print("MobSystem Ready.");
    }
    public override void _ExitTree()
    {
        _eventService.Unsubscribe<Init>(OnInit);
        _eventService.Unsubscribe<MobSpawnTimeout>(OnMobTimeout);
        _eventService.Unsubscribe<GameTimeout>(OnGameTimeout);
    }
    public override void _Process(double delta)
    {
        _deltaTime = delta;
    }
    public void Update()
    {
        if (!IsInitialized)
        {
            GD.PrintErr("MobSystem: Update called but system is not initialized.");
            return;
        }
        // Center mob spawners around player
        _mobSpawnPaths[0].Position = _playerRef.Position - new Vector2(150f, 150f);
        foreach (var mob in _activeMobs)
        {
            MobEntity mobEntity = mob;
            if (mobEntity.CurrentHealth == 0)
            {
                mobEntity.Hide();
                _activeMobs.Remove(mobEntity);
                mobEntity.Position = Vector2.Zero;
                mobEntity.LinearVelocity = Vector2.Zero;
                mobEntity.FrameSkipCounter = 0;
                mobEntity.CurrentHealth = mobEntity.Data.Stats.MaxHealth;
                continue;
            }
            // Slow down processing for off-screen mobs
            if (!mobEntity.Notifier2D.IsOnScreen())
            {
                mobEntity.FrameSkipCounter++;
                if (mobEntity.FrameSkipCounter > 5)
                {
                    mobEntity.FrameSkipCounter = 0;
                }
                else continue;
            }
            mobEntity.Sprite.Play("Move");
            _aiHandlers[mobEntity.Data.MovementType](mobEntity);
        }
    }
    // Event Handlers
    public void OnInit()
    {
        if (IsInitialized) return;
        _playerRef = GetTree().GetFirstNodeInGroup("player") as HeroEntity;
        _levelRef = GetTree().GetFirstNodeInGroup("level") as LevelEntity;
        BuildMobTable();
        BuildMobPool();
        BuildMobPaths();
        RegisterAIHandlers();
        IsInitialized = true;
    }
    private void OnMobTimeout()
    {
        if (_mobTable == null || _mobTable.Count == 0)
        {
            GD.PrintErr("MobSystem: OnMobTimeout called but mob data lookup is empty.");
            return;
        }
        float floor = (float)Math.Clamp(MathF.Floor(_gameElapsedTime / 60f), 0, _mobWeights - 5f);
        float spawnPick = _random.RandfRange(floor, _mobWeights);
        float cumulativeWeight = 0f;
        MobEntity mobInstance = null;
        foreach (var (mob, weight) in _mobTable)
        {
            cumulativeWeight += weight;
            if (spawnPick <= cumulativeWeight)
            {
                foreach (var pooledMob in _pooledMobs)
                {
                    if (!pooledMob.Visible && pooledMob.Data == mob)
                    {
                        mobInstance = pooledMob;
                        break;
                    }
                }
            }
        }
        byte pathIndex = (byte)_random.RandiRange(0, _mobSpawners.Length - 1);
        mobInstance.Position = _mobSpawners[pathIndex].GlobalPosition;
        mobInstance.Show();
        _activeMobs.Add(mobInstance);
        GD.Print($"MobSystem: Spawned mob '{mobInstance.Data.Info.Name}' at path {pathIndex}.");
    }
    private void OnGameTimeout()
    {
        _gameElapsedTime += 1f;
    }
    // Initialization Methods
    private void BuildMobPaths()
    {
        _mobSpawnPaths = new Path2D[5];
        _mobSpawners = new PathFollow2D[5];
        for (int i = 0; i < _mobSpawnPaths.Length; i++)
        {
            Path2D mobPath = new Path2D();
            PathFollow2D mobSpawner = new PathFollow2D();
            Curve2D curve = new Curve2D();
            mobPath.AddToGroup("mob_paths");
            _mobSpawnPaths[i] = mobPath;
            _mobSpawners[i] = mobSpawner;
            mobSpawner.Loop = true;
            mobPath.AddChild(mobSpawner);
            _levelRef.AddChild(mobPath);
        }
        // Small Circle
        _mobSpawnPaths[0].Curve = BuildCurve("circle", 300f);
        _mobSpawnPaths[0].Position = _playerRef.GlobalPosition - new Vector2(150f, 150f);
        // Large Circle
        _mobSpawnPaths[1].Curve = BuildCurve("circle", 500f);
        _mobSpawnPaths[1].Position = _playerRef.GlobalPosition - new Vector2(250f, 250f);
        // Hexagon
        _mobSpawnPaths[2].Curve = BuildCurve("hexagon", 400f);
        _mobSpawnPaths[2].Position = _playerRef.GlobalPosition - new Vector2(200f, 200f);
        // Diamond
        _mobSpawnPaths[3].Curve = BuildCurve("diamond", 350f);
        _mobSpawnPaths[3].Position = _playerRef.GlobalPosition - new Vector2(175f, 175f);
        // Star
        _mobSpawnPaths[4].Curve = BuildCurve("star", 400f);
        _mobSpawnPaths[4].Position = _playerRef.GlobalPosition - new Vector2(200f, 200f);
        GD.Print("MobSystem: Built mob spawn paths.");
    }
    private Curve2D BuildCurve(string shape, float size)
    {
        Curve2D curve = new Curve2D();
        switch (shape)
        {
            case "circle":
                int points = 20;
                for (int i = 0; i < points; i++)
                {
                    float angle = (Mathf.Pi * 2 / points) * i;
                    Vector2 point = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * size;
                    curve.AddPoint(point);
                }
                break;
            case "hexagon":
                for (int i = 0; i < 6; i++)
                {
                    float angle = (Mathf.Pi * 2 / 6) * i;
                    Vector2 point = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * size;
                    curve.AddPoint(point);
                }
                break;
            case "diamond":
                curve.AddPoint(new Vector2(0, -size));
                curve.AddPoint(new Vector2(size, 0));
                curve.AddPoint(new Vector2(0, size));
                curve.AddPoint(new Vector2(-size, 0));
                break;
            case "star":
                for (int i = 0; i < 5; i++)
                {
                    float outerAngle = (Mathf.Pi * 2 / 5) * i;
                    float innerAngle = outerAngle + (Mathf.Pi / 5);
                    Vector2 outerPoint = new Vector2(Mathf.Cos(outerAngle), Mathf.Sin(outerAngle)) * size;
                    Vector2 innerPoint = new Vector2(Mathf.Cos(innerAngle), Mathf.Sin(innerAngle)) * (size / 2);
                    curve.AddPoint(outerPoint);
                    curve.AddPoint(innerPoint);
                }
                break;
            default:
                GD.PrintErr($"MobSystem: BuildCurve called with unknown shape '{shape}'.");
                break;
        }
        return curve;
    }
    private void BuildMobPool()
    {
        if (_mobTable == null || _mobTable.Count == 0)
        {
            GD.PrintErr("MobSystem: BuildMobPool called but mob data lookup is empty.");
            return;
        }
        _pooledMobs.Clear();
        foreach (var (mob, weight) in _mobTable)
        {
            // Create instances inversely proportional to their spawn weight, this is to make rarer mobs less common in the pool
            int poolSize = Mathf.CeilToInt(400f / weight);
            for (int i = 0; i < poolSize; i++)
            {
                MobEntity mobInstance = CreateMobEntity(mob);
                mobInstance.Hide();
                AddChild(mobInstance);
                _pooledMobs.Add(mobInstance);
            }
        }
        GD.Print($"MobSystem: Built mob pool with {_pooledMobs.Count} total mobs.");
    }
    private void BuildMobTable()
    {
        if (_levelRef == null || _levelRef.Data == null)
        {
            GD.PrintErr("MobSystem: BuildMobTable called but LevelInstance or LevelInstance.Data is null.");
            return;
        }
        LevelData levelData = _levelRef.Data as LevelData;
        RandomNumberGenerator rnd = new RandomNumberGenerator();
        rnd.Randomize();
        _mobTable = new();
        foreach (var mob in levelData.MobTable.Mobs)
        {
            float spawnWeight = CalculateSpawnWeight(mob);
            if (_mobTable.Find(x => x.mob == mob).weight == spawnWeight)
            {
                spawnWeight += rnd.RandfRange(0.01f, 0.99f);
            }
            _mobTable.Add((mob, spawnWeight));
            _mobWeights += spawnWeight;
        }
        _mobTable.Sort((a, b) => b.weight.CompareTo(a.weight));
        GD.Print($"MobSystem: Built mob table with {_mobTable.Count} possible mobs for level");
    }
    private float CalculateSpawnWeight(MobData mobData)
    {
        float baseWeight = ((float)mobData.MetaData.Rarity + 1f) * 10f;
        float levelMultiplier = 10f * ((float)mobData.Level +1f);
        return (baseWeight * levelMultiplier) / 255f;
    }
    private MobEntity CreateMobEntity(MobData mobData)
    {
        MobEntity mobInstance = _mobTemplate.Instantiate<MobEntity>();
        mobInstance.Inject(mobData);
        return mobInstance;
    }
    // AI Handlers and Behaviors
    private void RegisterAIHandlers()
    {
        _aiHandlers = new()
        {
            { MobMovement.Stationary, HandleIdleAI },
            { MobMovement.CurvedDirection, HandleCurvedAI },
            { MobMovement.DashDirection, HandleDashAI },
            { MobMovement.PlayerAttracted, HandleAttractedAI },
            { MobMovement.RandomDirection, HandleRandomAI },
            { MobMovement.ZigZagSway, HandleZigZagAI },
            { MobMovement.CircleStrafe, HandleCircleStrafeAI }
        };
    }
    private void HandleIdleAI(MobEntity mobEntity)
    {
        mobEntity.Position += Vector2.Zero;
    }
    private void HandleCurvedAI(MobEntity mobEntity)
    {
        Vector2 directionToPlayer = (_playerRef.Position - mobEntity.Position).Normalized();
        directionToPlayer = directionToPlayer.Rotated((float)GD.RandRange(-0.05, 0.05));
        mobEntity.LinearVelocity = (mobEntity.LinearVelocity * 0.95f + directionToPlayer * mobEntity.Data.Stats.Speed * 0.05f) * (float)_deltaTime;
    }
    private void HandleDashAI(MobEntity mobEntity)
    {
        if (mobEntity.LinearVelocity >= Vector2.Zero)
            mobEntity.LinearVelocity -= mobEntity.LinearVelocity * 0.1f;
        Vector2 directionToPlayer = (_playerRef.Position - mobEntity.Position).Normalized();
        mobEntity.LinearVelocity = (directionToPlayer * mobEntity.Data.Stats.Speed * 1.5f) * (float)_deltaTime;
    }
    private void HandleAttractedAI(MobEntity mobEntity)
    {
        Vector2 directionToPlayer = (_playerRef.Position - mobEntity.Position).Normalized();
        mobEntity.LinearVelocity = (directionToPlayer * mobEntity.Data.Stats.Speed) * (float)_deltaTime;
    }
    private void HandleRandomAI(MobEntity mobEntity)
    {
        if (mobEntity.LinearVelocity.Length() < 1f)
        {
            float angle = (float)GD.RandRange(0, Mathf.Pi * 2);
            Vector2 randomDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            mobEntity.LinearVelocity = (randomDirection * mobEntity.Data.Stats.Speed) * (float)_deltaTime;
        }
        else
            mobEntity.LinearVelocity -= mobEntity.LinearVelocity * 0.05f;
    }
    private void HandleZigZagAI(MobEntity mobEntity)
    {
        Vector2 directionToPlayer = (_playerRef.Position - mobEntity.Position).Normalized();
        float zigZagOffset = Mathf.Sin(_gameElapsedTime * 5f) * 0.25f + (float)GD.RandRange(-0.3f, 0.3f);
        Vector2 perpendicular = new Vector2(-directionToPlayer.Y, directionToPlayer.X);
        Vector2 zigZagDirection = (directionToPlayer + perpendicular * zigZagOffset).Normalized();
        mobEntity.LinearVelocity = (zigZagDirection * mobEntity.Data.Stats.Speed) * (float)_deltaTime;
    }
    private void HandleCircleStrafeAI(MobEntity mobEntity)
    {
        Vector2 directionToPlayer = (_playerRef.Position - mobEntity.Position).Normalized();
        Vector2 perpendicular = new Vector2(-directionToPlayer.Y, directionToPlayer.X);
        float circleOffset = Mathf.Sin(_gameElapsedTime * 3f) * 0.5f;
        Vector2 strafeDirection = (directionToPlayer + perpendicular * circleOffset).Normalized();
        mobEntity.LinearVelocity = (strafeDirection * mobEntity.Data.Stats.Speed) * (float)_deltaTime;
    }
}