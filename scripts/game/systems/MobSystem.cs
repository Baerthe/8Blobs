namespace Game;

using Godot;
using Core;
using Entities;
using Game.Interface;
using Core.Interface;
using System.Collections.Generic;

public sealed partial class MobSystem : Node2D, IGameSystem
{
    public bool IsInitialized { get; private set; } = false;
    private List<(MobData mob, float weight)> _mobSpawnPool;
    private Dictionary<MobData, System.Action<MobEntity, MobData>> _aiHandlers;
    private HeroEntity _playerRef = null;
    private LevelEntity _levelRef = null;
    private Vector2 _offsetBetweenSpawnerAndPlayer;
    private Path2D _mobSpawnPath;
    private PathFollow2D _mobSpawner;
    private Vector2 _lastPlayerPosition;
    private PackedScene _genericMobScene;
    private float _grossMobWeight = 0f;
    private float _gameElapsedTime = 0f;
    // Dependency Services
    private IEventService _eventService;
    public override void _Ready()
    {
        GD.Print("MobSystem Present.");
        _eventService = CoreProvider.EventService();
        _eventService.Subscribe(OnInit);
        _eventService.Subscribe(OnMobTimeout);
        _eventService.Subscribe(OnGameTimeout);
    }
    public override void _ExitTree()
    {
        _eventService.Unsubscribe(OnInit);
        _eventService.Unsubscribe(OnMobTimeout);
        _eventService.Unsubscribe(OnGameTimeout);
    }
    public void OnInit()
    {
        if (IsInitialized) return;
        _playerRef = GetTree().GetFirstNodeInGroup("player") as HeroEntity;
        _levelRef = GetTree().GetFirstNodeInGroup("level") as LevelEntity;
        var levelData = _levelRef.Data as LevelData;
        //_genericMobScene = templates.MobTemplate;
        _aiHandlers = new Dictionary<MobData, System.Action<MobEntity, MobData>>();
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
        // Mob AI Update
        var activeMobs = GetTree().GetNodesInGroup("mobs");
        //TODO
    }
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
        foreach (var mob in levelData.MobTable)
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
        float baseWeight = ((float)mobData.Rarity + 1f) * 100f;
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
        if (_genericMobScene == null)
        {
            _genericMobScene = GD.Load<PackedScene>("res://scripts/game/entities/mobs/MobEntity.tscn");
        }
        MobEntity mobInstance = _genericMobScene.Instantiate<MobEntity>();
        mobInstance.Inject(mobData);
        return mobInstance;
    }
}