namespace Core.Interface;

using Godot;
using Mobs;

/// <summary>
/// Interface for the current game state.
/// </summary>
public interface IStateManager : ICore
{
    Player player { get; }
    Ui ui { get; }
    void UpdateScore(int points);
    int GetScore();
    void AddMobs(Mob[] mobs);
    void RemoveMob(Mob mob);
    void ClearMobs();
}