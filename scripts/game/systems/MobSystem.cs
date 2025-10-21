namespace Game;
using Godot;
using Entities;
using Game.Interface;
using System.Collections.Generic;
using Core;

public sealed partial class MobSystem : Node2D, IGameSystem
{
    public bool IsInitialized { get; private set; } = false;
    public Vector2 OffsetBetweenMobSpawnerAndPlayer { get; private set; }
    public Path2D MobSpawnPath { get; set; }
    public PathFollow2D MobSpawner { get; set; }
    public HeroEntity PlayerInstance { get; set; }
    public LevelEntity LevelInstance { get; set; }
    private List<(MobData mob, float weight)> _mobSpawnPool;
    private Dictionary<MobData, System.Action<MobEntity, MobData>> _aiHandlers;
    private List<MobEntity> _activeMobs = new();
    private Vector2 _lastPlayerPosition;
    private uint _maxLevel = 0;
    private float _maxTime = 600f;
    private float _grossMobWeight = 0f;
    private float _gameElapsedTime = 0f;
    public override void _Ready()
    {
        GD.Print("MobSystem Present.");
        GetParent<GameManager>().OnLevelLoad += (sender, args) =>
        {
            OnLevelLoad(args.LevelInstance, args.PlayerInstance);
        };
    }
    public void OnLevelLoad(LevelEntity levelInstance, HeroEntity playerInstance)
    {
        if (IsInitialized) return;
        PlayerInstance = playerInstance;
        var levelData = levelInstance.Data as LevelData;
        _maxLevel = levelData.MaxLevel;
        _maxTime = levelData.MaxTime;
        CoreProvider.GetClockService().MobSpawnTimeout += OnMobTimeout;
        CoreProvider.GetClockService().GameTimeout += OnGameTimeout;
        _aiHandlers = new Dictionary<MobData, System.Action<MobEntity, MobData>>();
        IsInitialized = true;
    }
    public void PauseMobs()
    {
        foreach (var mob in _activeMobs)
        {
            mob.SetProcess(false);
            mob.SetPhysicsProcess(false);
        }
    }
    public void ResumeMobs()
    {
        foreach (var mob in _activeMobs)
        {
            mob.SetProcess(true);
            mob.SetPhysicsProcess(true);
        }
    }
    public void Update()
    {
        OffsetBetweenMobSpawnerAndPlayer = PlayerInstance.Position - MobSpawnPath.Position;
        _lastPlayerPosition = PlayerInstance.Position;
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
        if (LevelInstance == null || LevelInstance.Data == null)
        {
            GD.PrintErr("MobSystem: BuildMobPool called but LevelInstance or LevelInstance.Data is null.");
            return;
        }
        LevelData levelData = LevelInstance.Data as LevelData;
        _mobSpawnPool = new();
        foreach (var mob in levelData.MobTable)
        {
            MobEntity mobInstance = ResourceLoader.Load<PackedScene>(mob.Entity.ResourcePath).Instantiate<MobEntity>();
            float spawnWeight = CalculateSpawnWeight(mobInstance.Data as MobData);
            while(_mobSpawnPool.Find(x => x.mob == mob).weight == spawnWeight)
            {
                spawnWeight += 0.32f; // Ensure unique weights
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
        int levelValue = (int)level;
        float progression = Mathf.Clamp(gameTime / _maxTime, 0f, 1f);
        float levelBonus = Mathf.Pow(progression, 2f) * levelValue;
        float levelPenalty = (1f - progression) * (_maxLevel - levelValue);
        return levelBonus + levelPenalty + 0.1f;
    }
}