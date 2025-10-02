namespace Core.Interface;

using Godot;
/// <summary>
/// Interface for GameManager to handle global game logic.
/// </summary>
public interface IGameManager
{
    void StartGame();
    void EndGame();
    void ResetGame();
    float GetTimeElapsed();
}