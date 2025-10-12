namespace Game;
using Godot;
using Entities;
using Game.Interface;
using System.Collections.Generic;
[GlobalClass]
public sealed partial class MobSystem : Node2D, IGameSystem
{
    public bool IsInitialized { get; private set; } = false;
    private Dictionary<MobEntity, MobData> _mobDataLookup;
    private Dictionary<MobEntity, System.Action<MobEntity, MobData>> _aiHandlers;
    private List<MobEntity> _activeMobs = new();
    private Node _parent;
    public override void _Ready()
    {
        if (IsInitialized) return;
        _parent = GetParent();
        _mobDataLookup = new Dictionary<MobEntity, MobData>();
        _aiHandlers = new Dictionary<MobEntity, System.Action<MobEntity, MobData>>();
        IsInitialized = true;
        GD.Print($"[MobSystem]: Initialized in {_parent.Name}");
    }
    public void Update()
    {
        
    }
}