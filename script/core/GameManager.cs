namespace Core;
using Core.Interface;
using System;
using Godot;
/// <summary>
/// A manager that contains the heartbeat pulse of the game, allowing decoupled update calls; also stores game data not directly related to the player. PlayerDataManager handles player-specific data relevant to the current level only.
/// This will also contain game specific settings when relevant.
/// </summary>
public sealed partial class GameManager : IGameManager
{
    public event Action PulseTimeout;
    public Vector2 OffsetBetweenPickupAndPlayer { get; private set; }
    public Vector2 OffsetBetweenMobAndPlayer { get; private set; }
    private Timer _pulseTimer;
    private Timer _gameTimer;
    private Timer _startingTimer;
    private Timer _mobSpawnTimer;
    private Timer _pickupSpawnTimer;
    public GameManager()
    {
        _pulseTimer = new Timer();
        _pulseTimer.WaitTime = 1f;
        _pulseTimer.OneShot = false;
        _pulseTimer.Timeout += OnPulseTimeout;
    }
    public void InitGame(Vector2 PickupOffset, Vector2 MobOffset)
    {
        OffsetBetweenPickupAndPlayer = PickupOffset;
        OffsetBetweenMobAndPlayer = MobOffset;
        StartPulseTimer();
    }
    public void ResetGame()
    {
        OffsetBetweenPickupAndPlayer = Vector2.Zero;
        OffsetBetweenMobAndPlayer = Vector2.Zero;
        StopPulseTimer();
    }
    private void StartPulseTimer() => _pulseTimer.Start();
    private void StopPulseTimer() => _pulseTimer.Stop();
    private void OnPulseTimeout() => PulseTimeout?.Invoke();
}
