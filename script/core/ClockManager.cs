namespace Core;
using Core.Interface;
using System;
using System.Collections.Generic;
using Godot;
public sealed partial class ClockManager : IClockManager
{
    public event Action PulseTimeout;
    public event Action SlowPulseTimeout;
    public Vector2 OffsetBetweenPickupAndPlayer { get; private set; }
    public Vector2 OffsetBetweenMobAndPlayer { get; private set; }
    private static Timer _pulseTimer;
    private static Timer _slowPulseTimer;
    private static Timer _gameTimer;
    private static Timer _startingTimer;
    private static Timer _mobSpawnTimer;
    private static Timer _pickupSpawnTimer;
    public ClockManager()
    {

    }
    public void InitGame(Vector2 PickupOffset, Vector2 MobOffset)
    {
        OffsetBetweenPickupAndPlayer = PickupOffset;
        OffsetBetweenMobAndPlayer = MobOffset;
        CreatePulseTimer();
        CreateSlowPulseTimer();
        CreateMobSpawnTimer();
        CreatePickupSpawnTimer();
        CreateGameTimer();
        CreateStartingTimer();
        StartTimers();
    }
    public void ResetGame()
    {
        OffsetBetweenPickupAndPlayer = Vector2.Zero;
        OffsetBetweenMobAndPlayer = Vector2.Zero;
        StopTimers();
    }
    public void PauseTimers()
    {
        _pulseTimer.Paused = true;
        _slowPulseTimer.Paused = true;
        _gameTimer.Paused = true;
        _startingTimer.Paused = true;
        _mobSpawnTimer.Paused = true;
        _pickupSpawnTimer.Paused = true;
    }
    public void ResumeTimers()
    {
        _pulseTimer.Paused = false;
        _slowPulseTimer.Paused = false;
        _gameTimer.Paused = false;
        _startingTimer.Paused = false;
        _mobSpawnTimer.Paused = false;
        _pickupSpawnTimer.Paused = false;
    }
    public void SetTimers(List<float> Timers)
    {
        if (Timers.Count != 4)
        {
            GD.PrintErr($"There are four timers to set; but we received {Timers.Count} timers. Did we forget one?");
            return;
        }
        if (_mobSpawnTimer == null) CreateMobSpawnTimer();
        if (_pickupSpawnTimer == null) CreatePickupSpawnTimer();
        if (_gameTimer == null) CreateGameTimer();
        if (_startingTimer == null) CreateStartingTimer();
        try
        {
            _mobSpawnTimer.WaitTime = Timers[0];
            _pickupSpawnTimer.WaitTime = Timers[1];
            _gameTimer.WaitTime = Timers[2];
            _startingTimer.WaitTime = Timers[3];
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Failed to set timers: {ex.Message}");
        }
    }
    private void StartTimers()
    {
        _pulseTimer?.Start();
        _slowPulseTimer?.Start();
        _mobSpawnTimer?.Start();
        _pickupSpawnTimer?.Start();
        _gameTimer?.Start();
        _startingTimer?.Start();
    }
    private void StopTimers()
    {
        _pulseTimer?.Stop();
        _slowPulseTimer?.Stop();
        _mobSpawnTimer?.Stop();
        _pickupSpawnTimer?.Stop();
        _gameTimer?.Stop();
        _startingTimer?.Stop();
    }
    private void OnPulseTimeout() => PulseTimeout?.Invoke();
    private void OnSlowPulseTimeout() => SlowPulseTimeout?.Invoke();
    private void CreatePulseTimer()
    {
        if (_pulseTimer != null) return;
        _pulseTimer = new Timer { WaitTime = 0.05f, OneShot = false, Autostart = false };
        _pulseTimer.Timeout += OnPulseTimeout;
    }
    private void CreateSlowPulseTimer()
    {
        if (_slowPulseTimer != null) return;
        _slowPulseTimer = new Timer { WaitTime = 1f, OneShot = false, Autostart = false };
        _slowPulseTimer.Timeout += () => SlowPulseTimeout?.Invoke();
    }
    private void CreateMobSpawnTimer()
    {
        if (_mobSpawnTimer != null) return;
        _mobSpawnTimer = new Timer { WaitTime = 5f, OneShot = false, Autostart = false };
        _mobSpawnTimer.Timeout += () => GD.Print("Mob Spawn Timer Timeout");
    }
    private void CreatePickupSpawnTimer()
    {
        _pickupSpawnTimer = new Timer { WaitTime = 10f, OneShot = false, Autostart = false };
        _pickupSpawnTimer.Timeout += () => GD.Print("Pickup Spawn Timer Timeout");
    }
    private void CreateGameTimer() => _gameTimer = new Timer { WaitTime = 60f, OneShot = true };
    private void CreateStartingTimer() => _startingTimer = new Timer { WaitTime = 3f, OneShot = true };
}
