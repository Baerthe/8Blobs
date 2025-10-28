namespace Game;

using Godot;
using Core;
using Entities;
using Game.Interface;
using Core.Interface;
using System.Collections.Generic;
/// <summary>
/// MobSystem is responsible for managing all mobile entities (mobs) within the game.
/// It handles their initialization, spawning, AI behavior, and updates during the game loop.
/// The system maintains a pool of mobs based on the current level's data and processes their actions based on predefined AI movement types.
/// </summary>
public sealed partial class MobSystem : Node2D, IGameSystem
{
    public bool IsInitialized { get; private set; } = false;
    private List<(MobData mob, float weight)> _mobSpawnPool;
    private Dictionary<MobMovement, System.Action<MobEntity>> _aiHandlers;
    private HeroEntity _playerRef = null;
    private LevelEntity _levelRef = null;
    private Vector2 _offsetBetweenSpawnerAndPlayer;
    private Path2D _mobSpawnPath;
    private PathFollow2D _mobSpawner;
    private Vector2 _lastPlayerPosition;
    private PackedScene _mobTemplate;
    private float _grossMobWeight = 0f;
    private float _gameElapsedTime = 0f;
    private double _deltaTime = 0.0;
    // Dependency Services
    private readonly IAudioService _audioService;
    private readonly IEventService _eventService;
    public MobSystem(PackedScene mobTemplate)
    {
        GD.Print("MobSystem: Initializing...");
        _audioService = CoreProvider.AudioService();
        _eventService = CoreProvider.EventService();
        _mobTemplate = mobTemplate;
    }
    public override void _Ready()
    {
        _eventService.Subscribe<Init>(OnInit);
        _eventService.Subscribe(OnMobTimeout);
        _eventService.Subscribe(OnGameTimeout);
        GD.Print("MobSystem Ready.");
    }
    public override void _ExitTree()
    {
        _eventService.Unsubscribe<Init>(OnInit);
        _eventService.Unsubscribe(OnMobTimeout);
        _eventService.Unsubscribe(OnGameTimeout);
    }
    public override void _Process(double delta)
    {
        _deltaTime = delta;
    }
    public void OnInit()
    {
        if (IsInitialized) return;
        _playerRef = GetTree().GetFirstNodeInGroup("player") as HeroEntity;
        _levelRef = GetTree().GetFirstNodeInGroup("level") as LevelEntity;
        IsInitialized = true;
    }
    public void PauseMobs()
    {
        var activeMobs = GetTree().GetNodesInGroup("mobs");
        foreach (var mob in activeMobs)
        {
            mob.SetProcess(false);
            mob.SetPhysicsProcess(false);
        }
    }
    public void ResumeMobs()
    {
        var activeMobs = GetTree().GetNodesInGroup("mobs");
        foreach (var mob in activeMobs)
        {
            mob.SetProcess(true);
            mob.SetPhysicsProcess(true);
        }
    }
    public void Update()
    {
        if (!IsInitialized)
        {
            GD.PrintErr("MobSystem: Update called but system is not initialized.");
            return;
        }
        _offsetBetweenSpawnerAndPlayer = _playerRef.Position - _mobSpawnPath.Position;
        _lastPlayerPosition = _playerRef.Position;
        var activeMobs = GetTree().GetNodesInGroup("mobs");
        foreach (var mob in activeMobs)
        {
            MobEntity mobEntity = mob as MobEntity;
            if (mobEntity.Attributes.CurrentHealth == 0)
            {
                //TODO: play mob death shader/effects/sound here, death queue etc.
                // Temporarily just free the mob
                mobEntity.QueueFree();
                continue;
            }
            // Slow down processing for off-screen mobs
            if (!mobEntity.Notifier2D.IsOnScreen())
            {
                mobEntity.Attributes.FrameSkipCounter++;
                if (mobEntity.Attributes.FrameSkipCounter > 5)
                {
                    mobEntity.Attributes.FrameSkipCounter = 0;
                } else
                    continue;
            }
            _aiHandlers[mobEntity.Data.MovementType](mobEntity);
        }
    }
    // Event Handlers
    private void OnMobTimeout()
    {
        if (_mobSpawnPool == null || _mobSpawnPool.Count == 0)
        {
            GD.PrintErr("MobSystem: OnMobTimeout called but mob data lookup is empty.");
            return;
        }
        //TODO: Implement spawning logic based on weights and game time
    }
    private void OnGameTimeout()
    {
        _gameElapsedTime += 1f; // Increment game time by 1 second, which should be the timeout of this event.
    }
    /// <summary>
    /// Builds the mob pool from the current level, sorts them by spawn weight, and prepares them for spawning by loading their scenes.
    /// </summary>
    private void BuildMobPool()
    {
        if (_levelRef == null || _levelRef.Data == null)
        {
            GD.PrintErr("MobSystem: BuildMobPool called but LevelInstance or LevelInstance.Data is null.");
            return;
        }
        LevelData levelData = _levelRef.Data as LevelData;
        RandomNumberGenerator rnd = new RandomNumberGenerator();
        rnd.Randomize();
        _mobSpawnPool = new();
        foreach (var mob in levelData.MobTable.Mobs)
        {
            MobEntity mobInstance = DuplicateMobEntity(mob);
            float spawnWeight = CalculateSpawnWeight(mobInstance.Data);
            if (_mobSpawnPool.Find(x => x.mob == mob).weight == spawnWeight)
            {
                spawnWeight += rnd.RandfRange(0.02f, 1.28f);
            }
            _mobSpawnPool.Add((mob, spawnWeight));
            mobInstance.SetProcess(false);
            mobInstance.SetPhysicsProcess(false);
            mobInstance.Hide();
            AddChild(mobInstance);
            _grossMobWeight += spawnWeight;
        }
        _mobSpawnPool.Sort((a, b) => b.weight.CompareTo(a.weight));
        GD.Print($"MobSystem: Built mob pool with {_mobSpawnPool.Count} mobs for level");
    }
    private float CalculateSpawnWeight(MobData mobData)
    {
        float baseWeight = ((float)mobData.MetaData.Rarity + 1f) * 100f;
        float modWeight = baseWeight / 255f;
        float levelMultiplier = 1f + ((float)mobData.Level) * 0.2f;
        return modWeight * levelMultiplier;
    }
    private float CalculateLevelCurve(MobLevel level, float gameTime)
    {
        LevelData levelData = _levelRef.Data as LevelData;
        int levelValue = (int)level;
        float progression = Mathf.Clamp(gameTime / levelData.MaxTime, 0f, 1f);
        float levelBonus = Mathf.Pow(progression, 2f) * levelValue;
        float levelPenalty = (1f - progression) * (levelData.MaxLevel - levelValue);
        return levelBonus + levelPenalty + 0.1f;
    }
    private MobEntity DuplicateMobEntity(MobData mobData)
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
        mobEntity.LinearVelocity = mobEntity.LinearVelocity * 0.95f + directionToPlayer * mobEntity.Data.Stats.Speed * 0.05f;
    }
    private void HandleDashAI(MobEntity mobEntity)
    {
        if (mobEntity.LinearVelocity >= Vector2.Zero)
            mobEntity.LinearVelocity -= mobEntity.LinearVelocity * 0.1f;
        Vector2 directionToPlayer = (_playerRef.Position - mobEntity.Position).Normalized();
        mobEntity.LinearVelocity = directionToPlayer * mobEntity.Data.Stats.Speed * 1.5f;
    }
    private void HandleAttractedAI(MobEntity mobEntity)
    {
        Vector2 directionToPlayer = (_playerRef.Position - mobEntity.Position).Normalized();
        mobEntity.LinearVelocity = directionToPlayer * mobEntity.Data.Stats.Speed;
    }
    private void HandleRandomAI(MobEntity mobEntity)
    {
        if (mobEntity.LinearVelocity.Length() < 1f)
        {
            float angle = (float)GD.RandRange(0, Mathf.Pi * 2);
            Vector2 randomDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            mobEntity.LinearVelocity = randomDirection * mobEntity.Data.Stats.Speed;
        }
        else
        {
            mobEntity.LinearVelocity -= mobEntity.LinearVelocity * 0.05f;
        }
    }
    private void HandleZigZagAI(MobEntity mobEntity)
    {
        Vector2 directionToPlayer = (_playerRef.Position - mobEntity.Position).Normalized();
        float zigZagOffset = Mathf.Sin(_gameElapsedTime * 5f) * 0.25f + (float)GD.RandRange(-0.3f, 0.3f);
        Vector2 perpendicular = new Vector2(-directionToPlayer.Y, directionToPlayer.X);
        Vector2 zigZagDirection = (directionToPlayer + perpendicular * zigZagOffset).Normalized();
        mobEntity.LinearVelocity = zigZagDirection * mobEntity.Data.Stats.Speed;
    }
    private void HandleCircleStrafeAI(MobEntity mobEntity)
    {
        Vector2 directionToPlayer = (_playerRef.Position - mobEntity.Position).Normalized();
        Vector2 perpendicular = new Vector2(-directionToPlayer.Y, directionToPlayer.X);
        float circleOffset = Mathf.Sin(_gameElapsedTime * 3f) * 0.5f;
        Vector2 strafeDirection = (directionToPlayer + perpendicular * circleOffset).Normalized();
        mobEntity.LinearVelocity = strafeDirection * mobEntity.Data.Stats.Speed;
    }
}