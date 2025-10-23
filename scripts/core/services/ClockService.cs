namespace Core;

using Godot;
using System;
using System.Collections.Generic;
using Core.Interface;
/// <summary>
/// Service that manages game timing and timers. Provides various timed events for game logic.
/// </summary>
public sealed class ClockService : IClockService
{
    public event Action PulseTimeout;
    public event Action SlowPulseTimeout;
    public event Action MobSpawnTimeout;
    public event Action ChestSpawnTimeout;
    public event Action GameTimeout;
    public event Action StartingTimeout;
    private Timer _pulseTimer;
    private Timer _slowPulseTimer;
    private Timer _gameTimer;
    private Timer _startingTimer;
    private Timer _mobSpawnTimer;
    private Timer _ChestSpawnTimer;
    private Dictionary<byte, Timer> _timers = new();
    private bool _isInitialized = false;
    // Timer Default Intervals
    private const float PulseInterval = 0.05f;        // 20 hrz (~1200 per minute)
    private const float SlowPulseInterval = 0.2f;     // 5 hrz (~300 per minute)
    private const float MobSpawnInterval = 5f;       // 0.2 hrz (~12 per minute)
    private const float ChestSpawnInterval = 10f;     // 0.1 hrz (~6 per minute)
    private const float GameInterval = 60f;          // 0.016 hrz (~1 per minute)
    private const float StartingInterval = 3f;       // OneShot (~3 seconds)
    public ClockService()
    {
        _isInitialized = false;
        GD.Print("ClockService created, early stage, init during main load.");
    }
    /// <summary>
    /// Initializes the ClockService and starts all timers.
    /// </summary>
    /// <param name="parent"></param>
    /// <exception cref="InvalidOperationException"></exception>
    /// <remarks>
    /// This method should be called once when the game starts from the main parent node; because it uses Godot Timers, not System.Timers.
    /// </remarks>
    public void InitGame(Node parent)
    {
        if (_isInitialized)
        {
            GD.PrintErr("ClockService is already initialized. InitGame should only be called once per game session.");
            return;
        }
        if (parent == null)
        {
            GD.PrintErr("Parent node is null. Cannot initialize ClockService.");
            throw new InvalidOperationException("ERROR 002: Parent node is null in ClockService. Timers cannot start.");
        }
        CreatePulseTimer();
        CreateSlowPulseTimer();
        CreateMobSpawnTimer();
        CreateChestSpawnTimer();
        CreateGameTimer();
        CreateStartingTimer();
        StartTimers(parent);
        GD.PrintRich("[color=#00ff88]ClockService initialized, late stage, and timers started.[/color]");
        _isInitialized = true;
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
    private void StartTimers(Node parent)
    {
        if (parent == null)
        {
            GD.PrintErr("Parent node is null. Cannot start timers.");
            throw new InvalidOperationException("ERROR 002: Parent node is null in ClockService. Timers cannot start.");
        }
        foreach (var timer in _timers.Values)
        {
            if (timer.GetParent() == null)
            {
                parent.AddChild(timer);
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
        timer.Timeout += () => onTimeout?.Invoke();
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
