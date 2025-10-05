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
    public void InitGame()
    {
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
        if (_pulseTimer.IsStopped() || _slowPulseTimer.IsStopped() || _mobSpawnTimer.IsStopped() || _pickupSpawnTimer.IsStopped() || _gameTimer.IsStopped() || _startingTimer.IsStopped())
        {
            GD.PrintErr("One or more timers failed to start! Something going on here.");
        }
    }
    private void StopTimers()
    {
        _pulseTimer?.Stop();
        _slowPulseTimer?.Stop();
        _mobSpawnTimer?.Stop();
        _pickupSpawnTimer?.Stop();
        _gameTimer?.Stop();
        _startingTimer?.Stop();
        if (_pulseTimer.IsStopped() == false || _slowPulseTimer.IsStopped() == false || _mobSpawnTimer.IsStopped() == false || _pickupSpawnTimer.IsStopped() == false || _gameTimer.IsStopped() == false || _startingTimer.IsStopped() == false)
        {
            GD.PrintErr("One or more timers failed to stop! Something going on here.");
        }
    }
    private void CreatePulseTimer()
    {
        if (_pulseTimer != null) return;
        _pulseTimer = new Timer { WaitTime = 0.05f, OneShot = false, Autostart = false };
        GD.Print("Pulse Timer created with WaitTime 0.05f (20hrz)");
        _pulseTimer.Timeout += () => PulseTimeout?.Invoke();
        if (_pulseTimer == null)
        {
            GD.PrintErr("Pulse Timer is null after creation!");
            throw new InvalidOperationException("ERROR 001:Pulse Timer failed to initialize in ClockManager.");
        }
    }
    private void CreateSlowPulseTimer()
    {
        if (_slowPulseTimer != null) return;
        _slowPulseTimer = new Timer { WaitTime = 0.2f, OneShot = false, Autostart = false };
        GD.Print("Slow Pulse Timer created with WaitTime 0.2f (5hrz)");
        _slowPulseTimer.Timeout += () => SlowPulseTimeout?.Invoke();
        if (_slowPulseTimer == null)
        {
            GD.PrintErr("Slow Pulse Timer is null after creation!");
            throw new InvalidOperationException("ERROR 002: Slow Pulse Timer failed to initialize in ClockManager.");
        }
    }
    private void CreateMobSpawnTimer()
    {
        if (_mobSpawnTimer != null) return;
        _mobSpawnTimer = new Timer { WaitTime = 5f, OneShot = false, Autostart = false };
        _mobSpawnTimer.Timeout += () => GD.Print("Mob Spawn Timer Timeout");
        if (_mobSpawnTimer == null)
        {
            GD.PrintErr("Mob Spawn Timer is null after creation!");
            throw new InvalidOperationException("ERROR 003: Mob Spawn Timer failed to initialize in ClockManager.");
        }
    }
    private void CreatePickupSpawnTimer()
    {
        if (_pickupSpawnTimer != null) return;
        _pickupSpawnTimer = new Timer { WaitTime = 10f, OneShot = false, Autostart = false };
        _pickupSpawnTimer.Timeout += () => GD.Print("Pickup Spawn Timer Timeout");
        if (_pickupSpawnTimer == null)
        {
            GD.PrintErr("Pickup Spawn Timer is null after creation!");
            throw new InvalidOperationException("ERROR 004: Pickup Spawn Timer failed to initialize in ClockManager.");
        }
    }
    private void CreateGameTimer()
    {
        if (_gameTimer != null) return;
        _gameTimer = new Timer { WaitTime = 60f, OneShot = true };
        _gameTimer.Timeout += () => GD.Print("Game Timer Timeout");
        if (_gameTimer == null)
        {
            GD.PrintErr("Game Timer is null after creation!");
            throw new InvalidOperationException("ERROR 005: Game Timer failed to initialize in ClockManager.");
        }
    }
    private void CreateStartingTimer()
    {
        if (_startingTimer != null) return;
        _startingTimer = new Timer { WaitTime = 3f, OneShot = true };
        _startingTimer.Timeout += () => GD.Print("Starting Timer Timeout");
        if (_startingTimer == null)
        {
            GD.PrintErr("Starting Timer is null after creation!");
            throw new InvalidOperationException("ERROR 006: Starting Timer failed to initialize in ClockManager.");
        }
    }
}
