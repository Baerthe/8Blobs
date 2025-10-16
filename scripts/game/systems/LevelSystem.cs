namespace Game;
using Godot;
using System;
using Game.Interface;
using Entities;
// TODO: Move this into GameManager, feels unnecessary to have a separate system for level management when game manager is orchestrating loaded levels and systems. Paths can be sent into the mob system and pickups should be handled by chest system.
public sealed partial class LevelSystem : Node2D, IGameSystem
{
    public bool IsInitialized { get; private set; } = false;
    public Camera2D camera { get; private set; }
    public Vector2 OffsetBetweenPickupAndPlayer { get; private set; }
    public Vector2 OffsetBetweenMobAndPlayer { get; private set; }
    public Path2D MobPath { get; set; }
    public PathFollow2D MobSpawner { get; set; }
    public Path2D PickupPath { get; set; }
    public PathFollow2D PickupSpawner { get; set; }
    private HeroEntity _playerInstance;
    public void Initialize(HeroEntity playerInstance)
    {
        if (IsInitialized)
        {
            GD.PrintErr("LevelSystem is already initialized.");
            throw new InvalidOperationException("ERROR 300: LevelSystem is already initialized. Cannot reinitialize.");
        }
        if (playerInstance == null)
        {
            GD.PrintErr("LevelSystem: Initialize called with null playerInstance.");
            throw new ArgumentNullException(nameof(playerInstance), "Player instance cannot be null.");
        }
        _playerInstance = playerInstance;
        camera = GetNode<Camera2D>("Camera2D");
        MobPath = GetNode<Path2D>("MobPath");
        MobSpawner = MobPath.GetNode<PathFollow2D>("MobSpawner");
        PickupPath = GetNode<Path2D>("PickupPath");
        PickupSpawner = PickupPath.GetNode<PathFollow2D>("PickupSpawner");
        if (camera == null)
        {
            GD.PrintErr("Camera node not found in LevelSystem");
            throw new InvalidOperationException("ERROR 301: Camera node not found in LevelSystem. Game cannot load.");
        }
        IsInitialized = true;
    }
    public void Update()
    {
                if (_playerInstance == null)
        {
            GD.PrintErr("Player node not found in LevelTool");
            throw new InvalidOperationException("ERROR 301: Player node not found in LevelTool. Game cannot load.");
        }
        if (camera == null)
        {
            GD.PrintErr("Camera node not found in LevelTool");
            throw new InvalidOperationException("ERROR 302: Camera node not found in LevelTool. Game cannot load.");
        }
        OffsetBetweenPickupAndPlayer = _playerInstance.Position - PickupPath.Position;
		OffsetBetweenMobAndPlayer = _playerInstance.Position - MobPath.Position;
    }
}