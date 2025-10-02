using Godot;
using System.Collections.Generic;

/// <summary>
/// A manager for handling game state and logic.
/// </summary>
public sealed class GameManager
{
    private Timer _pulseTimer;
    private Player _player;
    private Ui _ui;
    private int _score = 0;
	private double _pickupTimerDefaultWaitTime;
	private Vector2 _distantBetweenPickupAndPlayer;
	private Vector2 _distantBetweenMobAndPlayer;

    public GameManager(Player player, Ui ui)
    {
        _player = player;
        _ui = ui;
        _pulseTimer = new Timer();
        _pulseTimer.WaitTime = 2.5f;
        _pulseTimer.OneShot = false;
        _pulseTimer.Autostart = true;
        _pulseTimer.Timeout += OnPulseTimeout;
    }

    private void OnPulseTimeout()
    {
        // Handle pulse timeout logic here
    }
}
