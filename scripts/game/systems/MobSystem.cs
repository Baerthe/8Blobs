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
    private List<(MobEntity mob, float weight)> _mobDataLookup;
    private Dictionary<MobEntity, System.Action<MobEntity, MobData>> _aiHandlers;
    private List<MobEntity> _activeMobs = new();
    private Vector2 _lastPlayerPosition;
    private const int _maxLevel = 0;
    private const float _maxTime = 600f; // 10 minutes
    private float _grossMobWeight = 0f;
    private float _gameElapsedTime = 0f;
    public override void _Ready()
    {
        GD.Print("MobSystem Present.");
        GetParent<GameManager>().OnLevelLoad += (sender, args) =>
        {
            OnLevelLoad(args.PlayerInstance);
        };
    }
    public void OnLevelLoad(HeroEntity playerInstance)
    {
        if (IsInitialized) return;
        PlayerInstance = playerInstance;
        CoreProvider.GetClockService().MobSpawnTimeout += OnMobTimeout;
        CoreProvider.GetClockService().GameTimeout += OnGameTimeout;
        _aiHandlers = new Dictionary<MobEntity, System.Action<MobEntity, MobData>>();
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
        if (_mobDataLookup == null || _mobDataLookup.Count == 0)
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
        _mobDataLookup = new();
        foreach (var mob in LevelInstance.Data.MobTable)
        {
            MobEntity mobInstance = mob.Instantiate<MobEntity>();
            float spawnWeight = CalculateSpawnWeight(mobInstance.Data);
            while(_mobDataLookup.Find(x => x.mob == mobInstance).weight == spawnWeight)
            {
                spawnWeight += 0.32f; // Ensure unique weights
            }
            _mobDataLookup.Add((mobInstance, spawnWeight));
            mobInstance.SetProcess(false);
            mobInstance.SetPhysicsProcess(false);
            mobInstance.Hide();
            AddChild(mobInstance);
            _grossMobWeight += spawnWeight;
        }
        _mobDataLookup.Sort((a, b) => b.weight.CompareTo(a.weight));
        GD.Print($"MobSystem: Built mob pool with {_mobDataLookup.Count} mobs for level");
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