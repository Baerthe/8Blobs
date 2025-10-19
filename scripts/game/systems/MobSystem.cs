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
    private Dictionary<uint, MobEntity> _mobDataLookup;
    private uint _maxSpawnWeight = 0;
    private uint _lowestSpawnWeight = 0;
    private Dictionary<MobEntity, System.Action<MobEntity, MobData>> _aiHandlers;
    private List<MobEntity> _activeMobs = new();
    private Vector2 _lastPlayerPosition;
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
        int rand = GD.RandRange(0, _mobDataLookup.Count);
        MobEntity mob = _mobDataLookup[rand].Instantiate();
        MobSpawner.ProgressRatio = GD.Randf();
    }
    /// <summary>
    /// Handles the game timeout event to adjust mob spawn weights over time, increasing the difficulty by forcing higher weight mobs to spawn.
    /// </summary>
    private void OnGameTimeout()
    {
        _lowestSpawnWeight++;
        if (_lowestSpawnWeight >= _maxSpawnWeight)
        {
            _lowestSpawnWeight = _maxSpawnWeight - 4;
        }
    }
    /// <summary>
    /// Builds the mob pool for the current level based on the level's mob table, sorting mobs by their spawn weight derived from their rarity and level.
    /// </summary>
    private void BuildMobPool()
    {
        _mobDataLookup = new();
        foreach (var mob in LevelInstance.Data.MobTable)
        {
            MobEntity mobInstance = mob.Instantiate<MobEntity>();
            uint spawnWeight = (uint)mobInstance.Data.Rarity * 2 + (uint)mobInstance.Data.Level * 4;
            while (_mobDataLookup.ContainsKey(spawnWeight))
            {
                spawnWeight++;
            }
            _mobDataLookup[spawnWeight] = mobInstance;
        }
        // move all values down to fill any gaps in the dictionary keys
        uint currentKey = 0;
        foreach (var key in new List<uint>(_mobDataLookup.Keys))
        {
            if (key != currentKey)
            {
                _mobDataLookup[currentKey] = _mobDataLookup[key];
                _mobDataLookup.Remove(key);
            }
            currentKey++;
        }
    }
}