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
    private List<MobEntity> _mobDataLookup;
    private Dictionary<MobEntity, System.Action<MobEntity, MobData>> _aiHandlers;
    private List<MobEntity> _activeMobs = new();
    private Vector2 _lastPlayerPosition;
    private ulong _gameElapsedTime = 0;
    public override void _Ready()
    {
        if (IsInitialized) return;
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
        // Build weighted random selection
        ushort totalWeight = 0;
        foreach (MobEntity mob in _mobDataLookup)
        {
            totalWeight += (ushort)CalculateSpawnWeight(mob.Data);
        }
        //TODO: Implement spawning logic based on weights and game time
    }
    private void OnGameTimeout()
    {
        _gameElapsedTime += 1;
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
            _mobDataLookup.Add(mobInstance);
            mobInstance.SetProcess(false);
            mobInstance.SetPhysicsProcess(false);
            mobInstance.Hide();
            AddChild(mobInstance);
        }
        // Sort mobs by spawn weight descending
        _mobDataLookup.Sort((a, b) => CalculateSpawnWeight(b.Data).CompareTo(CalculateSpawnWeight(a.Data)));
        GD.Print($"MobSystem: Built mob pool with {_mobDataLookup.Count} mobs for level");
    }
    private float CalculateSpawnWeight(MobData mobData)
    {
        float baseWeight = ((float)mobData.Rarity + 1f) * 100f;
        float modWeight = baseWeight / 255f;
        float levelMultiplier = 1f + ((float)mobData.Level) * 0.2f;
        return modWeight * levelMultiplier;
    }
}