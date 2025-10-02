namespace Core.Interface;

using Godot;
/// <summary>
/// Interface for GameManager to handle global game logic.
/// </summary>
public interface IGameManager
{
    void InitGame();
    void ResetGame();
    void MenuShow();
    void MenuHide();
    void StartPulseTimer();
    void StopPulseTimer();
}