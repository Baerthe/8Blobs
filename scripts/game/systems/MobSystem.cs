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
    private List<(MobData mob, float weight)> _mobTable;
    private Dictionary<MobMovement, System.Action<MobEntity>> _aiHandlers;
    private List<MobEntity> _pooledMobs = new();
    private List<MobEntity> _activeMobs = new();
    private HeroEntity _playerRef = null;
    private LevelEntity _levelRef = null;
    private Vector2 _offsetBetweenSpawnerAndPlayer;
    private Path2D _mobSpawnPath; // TODO: We should have multiple spawn paths for more dynamic spawning and move these to a subclass
    private PathFollow2D _mobSpawner;
    private Vector2 _lastPlayerPosition;
    private PackedScene _mobTemplate;
    private float _grossMobWeight = 0f;
    private float _gameElapsedTime = 0f;
    private double _deltaTime = 0.0;
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
        _offsetBetweenSpawnerAndPlayer = _playerRef.Position - _mobSpawnPath.Position;
        _lastPlayerPosition = _playerRef.Position;
        foreach (var mob in _activeMobs)
        {
            MobEntity mobEntity = mob;
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
        //TODO: Implement spawning logic based on weights and game time
    }
    private void OnGameTimeout()
    {
        _gameElapsedTime += 1f; // Increment game time by 1 second, which should be the timeout of this event.
    }
    // Initialization Methods
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
            float poolSize = Mathf.CeilToInt(weight * 10f);
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
            _grossMobWeight += spawnWeight;
        }
        _mobTable.Sort((a, b) => b.weight.CompareTo(a.weight));
        GD.Print($"MobSystem: Built mob table with {_mobTable.Count} possible mobs for level");
    }
    private float CalculateSpawnWeight(MobData mobData)
    {
        float baseWeight = ((float)mobData.MetaData.Rarity + 1f) * 10f;
        float levelMultiplier = 10f * ((float)mobData.Level);
        return (baseWeight * levelMultiplier) / 255f;
    }
    private MobEntity CreateMobEntity(MobData mobData)
    {
        MobEntity mobInstance = _mobTemplate.Instantiate<MobEntity>();
        mobInstance.Inject(mobData);
        return mobInstance;
    }
    // Mob Spawning
    private float CalculateLevelCurve(MobLevel level, float gameTime)
    {
        LevelData levelData = _levelRef.Data as LevelData;
        int levelValue = (int)level;
        float progression = Mathf.Clamp(gameTime / levelData.MaxTime, 0f, 1f);
        float levelBonus = Mathf.Pow(progression, 2f) * levelValue;
        float levelPenalty = (1f - progression) * (levelData.MaxLevel - levelValue);
        return levelBonus + levelPenalty;
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