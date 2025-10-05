namespace Core;
using Core.Interface;
using System;
using System.Collections.Generic;
using Godot;
public sealed partial class ClockManager : IClockManager
{
    public event Action PulseTimeout;
    public event Action SlowPulseTimeout;
    public event Action MobSpawnTimeout;
    public event Action PickupSpawnTimeout;
    public event Action GameTimeout;
    public event Action StartingTimeout;
    public Vector2 OffsetBetweenPickupAndPlayer { get; private set; }
    public Vector2 OffsetBetweenMobAndPlayer { get; private set; }
    private static Timer _pulseTimer;
    private static Timer _slowPulseTimer;
    private static Timer _gameTimer;
    private static Timer _startingTimer;
    private static Timer _mobSpawnTimer;
    private static Timer _pickupSpawnTimer;
    private static Dictionary<byte, Timer> _timers = new();
    public ClockManager()
    {
        CreatePulseTimer();
        CreateSlowPulseTimer();
        CreateMobSpawnTimer();
        CreatePickupSpawnTimer();
        CreateGameTimer();
        CreateStartingTimer();
    }
    public void InitGame(Node parent)
    {
        StartTimers(parent);
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
            _pickupSpawnTimer.WaitTime = Timers[1];
            _gameTimer.WaitTime = Timers[2];
            _startingTimer.WaitTime = Timers[3];
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
            throw new InvalidOperationException("ERROR 002: Parent node is null in ClockManager. Timers cannot start.");
        }
        foreach (var timer in _timers.Values)
        {
            if (timer.GetParent() == null)
            {
                parent.AddChild(timer);
            }
            timer.Start();
        }
        if (_pulseTimer.IsStopped() || _slowPulseTimer.IsStopped() || _mobSpawnTimer.IsStopped() || _pickupSpawnTimer.IsStopped() || _gameTimer.IsStopped() || _startingTimer.IsStopped())
        {
            GD.PrintErr($"One or more timers failed to start! Pulse: {_pulseTimer.IsStopped()}, SlowPulse: {_slowPulseTimer.IsStopped()}, MobSpawn: {_mobSpawnTimer.IsStopped()}, PickupSpawn: {_pickupSpawnTimer.IsStopped()}, Game: {_gameTimer.IsStopped()}, Starting: {_startingTimer.IsStopped()}");
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
        if (_pulseTimer.IsStopped() == false || _slowPulseTimer.IsStopped() == false || _mobSpawnTimer.IsStopped() == false || _pickupSpawnTimer.IsStopped() == false || _gameTimer.IsStopped() == false || _startingTimer.IsStopped() == false)
        {
            GD.PrintErr($"One or more timers failed to stop! Pulse: {_pulseTimer.IsStopped()}, SlowPulse: {_slowPulseTimer.IsStopped()}, MobSpawn: {_mobSpawnTimer.IsStopped()}, PickupSpawn: {_pickupSpawnTimer.IsStopped()}, Game: {_gameTimer.IsStopped()}, Starting: {_startingTimer.IsStopped()}");
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
            throw new InvalidOperationException($"ERROR 001: Timer failed to initialize in ClockManager. Sender: {sender}");
        }
        _timers.Add((byte)timer.GetHashCode(), timer);
        return timer;
    }
    private void CreatePulseTimer()
    {
        if (_pulseTimer != null) return;
        _pulseTimer = BuildTimer(0.05f, false, false, () => PulseTimeout?.Invoke(), this);
        GD.Print("Pulse Timer created with WaitTime 0.05f (20hrz)");
    }
    private void CreateSlowPulseTimer()
    {
        if (_slowPulseTimer != null) return;
        _slowPulseTimer = BuildTimer(0.2f, false, false, () => SlowPulseTimeout?.Invoke(), this);
        GD.Print("Slow Pulse Timer created with WaitTime 0.2f (5hrz)");
    }
    private void CreateMobSpawnTimer()
    {
        if (_mobSpawnTimer != null) return;
        _mobSpawnTimer = BuildTimer(5f, false, false, () => MobSpawnTimeout?.Invoke(), this);
        GD.Print("Mob Spawn Timer created with WaitTime 5f");
    }
    private void CreatePickupSpawnTimer()
    {
        if (_pickupSpawnTimer != null) return;
        _pickupSpawnTimer = BuildTimer(10f, false, false, () => PickupSpawnTimeout?.Invoke(), this);
        GD.Print("Pickup Spawn Timer created with WaitTime 10f");
    }
    private void CreateGameTimer()
    {
        if (_gameTimer != null) return;
        _gameTimer = BuildTimer(60f, true, false, () => GameTimeout?.Invoke(), this);
        GD.Print("Game Timer created with WaitTime 60f (OneShot)");
    }
    private void CreateStartingTimer()
    {
        if (_startingTimer != null) return;
        _startingTimer = BuildTimer(3f, true, false, () => StartingTimeout?.Invoke(), this);
        GD.Print("Starting Timer created with WaitTime 3f (OneShot)");
    }
}
