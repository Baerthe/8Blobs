namespace Core.Interface;
using System;
using System.Collections.Generic;
using Godot;
/// <summary>
/// Interface for the ClockManager; this manager that contains the heartbeat pulse of the game, allowing decoupled update calls; also stores game data not directly related to the player. A box of clocks!
/// </summary>
public interface IClockManager
{
    /// <summary>
    /// Event triggered on each heartbeat pulse timeout.
    /// </summary>
    event Action PulseTimeout;
    Vector2 OffsetBetweenPickupAndPlayer { get; }
    Vector2 OffsetBetweenMobAndPlayer { get; }
    /// <summary>
    /// Initializes the game state with offsets for pickup and mob spawning relative to the player. Starts heartbeat pulse timer.
    /// </summary>
    /// <param name="PickupOffset"></param>
    /// <param name="MobOffset"></param>
    void InitGame(Vector2 PickupOffset, Vector2 MobOffset);
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