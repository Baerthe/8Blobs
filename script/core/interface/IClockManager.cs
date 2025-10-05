namespace Core.Interface;
using System;
using System.Collections.Generic;
using Godot;
/// <summary>
/// Interface for the ClockManager; this manager that contains the heartbeat pulse of the game, allowing decoupled update calls; also stores game data not directly related to the player. A box of clocks!
/// </summary>
/// <remarks>
/// Classes can (like and) subscribe to the PulseTimeout and SlowPulseTimeout events to get regular update calls, avoiding the need for a monolithic update loop. By default the pulse is set to 20hrz (0.05s) and the slow pulse to 5hrz (0.2s). These can be adjusted in the ClockManager, directly, if needed.
/// Things like Mob spawn rate and pickup spawn rate can be managed seprate from score and game time, etc. This allows for differences in levels, increase or decrease in difficulty, etc. without needing to change the core game loop.
/// </remarks>
public interface IClockManager
{
    /// <summary>
    /// Event triggered on each heartbeat pulse timeout.
    /// </summary>
    event Action PulseTimeout;
    /// <summary>
    /// Event triggered on each slow heartbeat pulse timeout.
    /// </summary>
    event Action SlowPulseTimeout;
    /// <summary>
    /// Event triggered when it's time to spawn a new mob.
    /// </summary>
    event Action MobSpawnTimeout;
    /// <summary>
    /// Event triggered when it's time to spawn a new pickup item.
    /// </summary>
    event Action PickupSpawnTimeout;
    /// <summary>
    /// Event triggered when the game timer runs out.
    /// </summary>
    event Action GameTimeout;
    /// <summary>
    /// Event triggered when the starting timer runs out.
    /// </summary>
    event Action StartingTimeout;
    /// <summary>
    /// Pauses all active timers in the game manager.
    /// </summary>
    void PauseTimers();
    /// <summary>
    /// Resumes all paused timers in the game manager.
    /// </summary>
    void ResumeTimers();
    /// <summary>
    /// Resets the game state, clearing offsets and stopping the heartbeat pulse timer.
    /// </summary>
    void ResetGame();
    /// <summary>
    /// Sends over the default timer times from the main scene or passes level specific timers.
    /// </summary>
    /// <param name="MobSpawnTime"></param>
    /// <param name="PickupSpawnTime"></param>
    /// <param name="ScoreTime"></param>
    /// <param name="StartingTime"></param>
    /// <remarks>
    /// This is to avoid hardcoding timer values in the ClockManager, allowing flexibility for different levels or game modes.
    /// </remarks>
    void SetTimers(List<float> Timers);
}