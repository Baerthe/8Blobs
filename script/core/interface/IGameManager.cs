namespace Core.Interface;
using System;
using Godot;
/// <summary>
/// Interface for the GameManager; this manager that contains the heartbeat pulse of the game, allowing decoupled update calls; also stores game data not directly related to the player. PlayerDataManager handles player-specific data relevant to the current level only.
/// </summary>
public interface IGameManager
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
    /// Resets the game state, clearing offsets and stopping the heartbeat pulse timer.
    /// </summary>
    void ResetGame();
}