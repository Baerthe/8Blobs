namespace Game;
using Godot;
using Entities;
using Game.Interface;
using System.Collections.Generic;
public sealed partial class MobSystem : Node2D, IGameSystem
{
    public bool IsInitialized { get; private set; } = false;
    public Vector2 OffsetBetweenMobSpawnerAndPlayer { get; private set; }
    public Path2D MobSpawnPath { get; set; }
    public PathFollow2D MobSpawner { get; set; }
    private Dictionary<MobEntity, MobData> _mobDataLookup;
    private Dictionary<MobEntity, System.Action<MobEntity, MobData>> _aiHandlers;
    private List<MobEntity> _activeMobs = new();
    public override void _Ready()
    {
        if (IsInitialized) return;
        _mobDataLookup = new Dictionary<MobEntity, MobData>();
        _aiHandlers = new Dictionary<MobEntity, System.Action<MobEntity, MobData>>();
        IsInitialized = true;
    }
    public void Update()
    {
        OffsetBetweenMobSpwanerAndPlayer = _playerInstance.Position - MobPath.Position;
    }
}