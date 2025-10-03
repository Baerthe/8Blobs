namespace Core.Interface;
using System;
/// <summary>
/// Interface for GameManager to handle global game logic.
/// </summary>
public interface IGameManager
{
    event Action PulseTimeout;
    void InitGame();
    void ResetGame();
    void MenuShow();
    void MenuHide();
    void StartPulseTimer();
    void StopPulseTimer();
}