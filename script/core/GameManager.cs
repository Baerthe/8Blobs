namespace Core;
using Core.Interface;
using System;
using Godot;
/// <summary>
/// A manager for handling game state and logic.
/// </summary>
public sealed partial class GameManager : IGameManager
{
    public event Action PulseTimeout;
    public Vector2 OffsetBetweenPickupAndPlayer { get; private set; }
    public Vector2 OffsetBetweenMobAndPlayer { get; private set; }
    private Timer _pulseTimer;
    private Ui _ui;
    private double _pickupTimerDefaultWaitTime;
    public GameManager(Ui ui, Vector2 offsetBetweenPickupAndPlayer, Vector2 offsetBetweenMobAndPlayer)
    {
        _ui = ui;
        OffsetBetweenPickupAndPlayer = offsetBetweenPickupAndPlayer;
        OffsetBetweenMobAndPlayer = offsetBetweenMobAndPlayer;
        _pulseTimer = new Timer();
        _pulseTimer.WaitTime = 1f;
        _pulseTimer.OneShot = false;
        _pulseTimer.Timeout += OnPulseTimeout;
    }
    public void InitGame()
    {

    }
    public void ResetGame()
    {

    }
    public void MenuShow()
    {
        _ui.Visible = true;
    }
    public void MenuHide()
    {
        _ui.Visible = false;
    }
    public void SetPickupTimerDefaultWaitTime(double time) => _pickupTimerDefaultWaitTime = time;
    public void StartPulseTimer() => _pulseTimer.Start();
    public void StopPulseTimer() => _pulseTimer.Stop();
    private void OnPulseTimeout() => PulseTimeout?.Invoke();
}
