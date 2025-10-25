namespace Game;

using Godot;
using Core;
using System;
using System.Collections.Generic;
using Core.Interface;
using Game.Interface;
/// <summary>
/// ClockSystem manages the game's timing mechanisms, including pulse, slow pulse, mob spawn, chest spawn, game, and starting timers. It integrates with the EventService to provide timed events that other game systems can subscribe to.
/// </summary>
public partial class ClockSystem : Node2D, IGameSystem
{
    public bool IsInitialized { get; private set; } = false;
    private Timer _pulseTimer;
    private Timer _slowPulseTimer;
    private Timer _gameTimer;
    private Timer _startingTimer;
    private Timer _mobSpawnTimer;
    private Timer _ChestSpawnTimer;
    private Dictionary<byte, Timer> _timers = new();
    // Timer Default Intervals
    private const float PulseInterval = 0.05f;        // 20 hrz (~1200 per minute)
    private const float SlowPulseInterval = 0.2f;     // 5 hrz (~300 per minute)
    private const float MobSpawnInterval = 5f;       // 0.2 hrz (~12 per minute)
    private const float ChestSpawnInterval = 10f;     // 0.1 hrz (~6 per minute)
    private const float GameInterval = 60f;          // 0.016 hrz (~1 per minute)
    private const float StartingInterval = 3f;       // OneShot (~3 seconds)
    // Dependency Services
    private IEventService _eventService;
    public void Init()
    {
        if (IsInitialized)
        {
            GD.PrintErr("ClockService is already initialized. InitGame should only be called once per game session.");
            return;
        }
        _eventService = CoreProvider.EventService();
        CreatePulseTimer();
        CreateSlowPulseTimer();
        CreateMobSpawnTimer();
        CreateChestSpawnTimer();
        CreateGameTimer();
        CreateStartingTimer();
        GD.PrintRich("[color=#00ff88]ClockService initialized, late stage, and timers started.[/color]");
        IsInitialized = true;
    }
    public void ResetGame()
    {
        StopTimers();
    }
    public void PauseTimers()
    {
        foreach (var timer in _timers.Values)
        {
            timer.Paused = true;
        }
    }
    public void ResumeTimers()
    {
        foreach (var timer in _timers.Values)
        {
            timer.Paused = false;
        }
    }
    public void SetTimers(List<float> Timers)
    {
        if (Timers.Count != 4)
        {
            GD.PrintErr($"There are four timers to set; but we received {Timers.Count} timers. Did we forget one?");
            return;
        }
        try
        {
            _mobSpawnTimer.WaitTime = Timers[0];
            GD.Print($"Mob Spawn Timer set to {Timers[0]} seconds.");
            _ChestSpawnTimer.WaitTime = Timers[1];
            GD.Print($"Pickup Spawn Timer set to {Timers[1]} seconds.");
            _gameTimer.WaitTime = Timers[2];
            GD.Print($"Game Timer set to {Timers[2]} seconds.");
            _startingTimer.WaitTime = Timers[3];
            GD.Print($"Starting Timer set to {Timers[3]} seconds.");
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Failed to set timers: {ex.Message}");
        }
    }
    /// <summary>
    /// Starts all timers. Used when initializing the game.
    /// </summary>
    private void StartTimers()
    {
        foreach (var timer in _timers.Values)
        {
            if (timer.GetParent() == null)
            {
                AddChild(timer);
            }
            timer.Start();
        }
        if (_pulseTimer.IsStopped() || _slowPulseTimer.IsStopped() || _mobSpawnTimer.IsStopped() || _ChestSpawnTimer.IsStopped() || _gameTimer.IsStopped() || _startingTimer.IsStopped())
        {
            GD.PrintErr($"One or more timers failed to start! Pulse: {_pulseTimer.IsStopped()}, SlowPulse: {_slowPulseTimer.IsStopped()}, MobSpawn: {_mobSpawnTimer.IsStopped()}, ChestSpawn: {_ChestSpawnTimer.IsStopped()}, Game: {_gameTimer.IsStopped()}, Starting: {_startingTimer.IsStopped()}");
        }
    }
    /// <summary>
    /// Stops all timers. Used when resetting the game.
    /// </summary>
    private void StopTimers()
    {
        foreach (var timer in _timers.Values)
        {
            timer.Stop();
        }
        if (_pulseTimer.IsStopped() == false || _slowPulseTimer.IsStopped() == false || _mobSpawnTimer.IsStopped() == false || _ChestSpawnTimer.IsStopped() == false || _gameTimer.IsStopped() == false || _startingTimer.IsStopped() == false)
        {
            GD.PrintErr($"One or more timers failed to stop! Pulse: {_pulseTimer.IsStopped()}, SlowPulse: {_slowPulseTimer.IsStopped()}, MobSpawn: {_mobSpawnTimer.IsStopped()}, ChestSpawn: {_ChestSpawnTimer.IsStopped()}, Game: {_gameTimer.IsStopped()}, Starting: {_startingTimer.IsStopped()}");
        }
    }
    /// <summary>
    /// Builds a Timer with the specified parameters and attaches the onTimeout action to its Timeout signal.
    /// </summary>
    /// <param name="waitTime"></param>
    /// <param name="oneShot"></param>
    /// <param name="autostart"></param>
    /// <param name="onTimeout"></param>
    /// <param name="sender"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">Thrown if the Timer fails to initialize. Lists which createTimer method was called.</exception>
    private Timer BuildTimer(float waitTime, bool oneShot, bool autostart, Action onTimeout, Object sender)
    {
        Timer timer = new Timer
        {
            WaitTime = waitTime,
            OneShot = oneShot,
            Autostart = autostart
        };
        timer.Timeout += () => onTimeout();
        if (timer == null)
        {
            GD.PrintErr("Timer is null after creation!");
            throw new InvalidOperationException($"ERROR 001: Timer failed to initialize in ClockService. Sender: {sender}");
        }
        _timers.Add((byte)timer.GetHashCode(), timer);
        return timer;
    }
    private void CreatePulseTimer()
    {
        if (_pulseTimer != null) return;
        _pulseTimer = BuildTimer(PulseInterval, false, false, () => PulseTimeout?.Invoke(), this);
        GD.Print("Pulse Timer created with WaitTime 0.05f (20hrz), ~1200 per minute");
    }
    private void CreateSlowPulseTimer()
    {
        if (_slowPulseTimer != null) return;
        _slowPulseTimer = BuildTimer(SlowPulseInterval, false, false, () => SlowPulseTimeout?.Invoke(), this);
        GD.Print("Slow Pulse Timer created with WaitTime 0.2f (5hrz), ~300 per minute");
    }
    private void CreateMobSpawnTimer()
    {
        if (_mobSpawnTimer != null) return;
        _mobSpawnTimer = BuildTimer(MobSpawnInterval, false, false, () => MobSpawnTimeout?.Invoke(), this);
        GD.Print("Mob Spawn Timer created with WaitTime 5f (0.2hrz), ~12 per minute");
    }
    private void CreateChestSpawnTimer()
    {
        if (_ChestSpawnTimer != null) return;
        _ChestSpawnTimer = BuildTimer(ChestSpawnInterval, false, false, () => ChestSpawnTimeout?.Invoke(), this);
        GD.Print("Pickup Spawn Timer created with WaitTime 10f (0.1hrz), ~6 per minute");
    }
    private void CreateGameTimer()
    {
        if (_gameTimer != null) return;
        _gameTimer = BuildTimer(GameInterval, false, false, () => GameTimeout?.Invoke(), this);
        GD.Print("Game Timer created with WaitTime 60f (0.016hrz), ~1 per minute");
    }
    private void CreateStartingTimer()
    {
        if (_startingTimer != null) return;
        _startingTimer = BuildTimer(StartingInterval, true, false, () => StartingTimeout?.Invoke(), this);
        GD.Print("Starting Timer created with WaitTime 3f (OneShot), ~3 seconds");
    }
}
