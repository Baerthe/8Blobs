namespace Core;
using Core.Interface;
using Godot;
using System;
public sealed class PrefService : IPrefService
{
    private bool _isInitialized;
    public PrefService()
    {
        _isInitialized = false;
        Initialize();
    }
    private void Initialize()
    {
        if (_isInitialized)
        {
            GD.PrintErr("PrefService is already initialized. Initialize should only be called once per game session.");
            return;
        }
        _isInitialized = true;
        GD.PrintRich("[color=#00ff88]PrefService initialized.[/color]");
    }
}