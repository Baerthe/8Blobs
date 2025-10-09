namespace Core;

using Godot;
using Core.Interface;
/// <summary>
/// Manages user save data.
/// </summary>
public class SaveManager : ISaveManager
{
    private bool _isInitialized;
    public SaveManager()
    {
        _isInitialized = false;
        Initilize();
    }
    private void Initilize()
    {
        if (_isInitialized)
        {
            GD.PrintErr("SaveManager is already initialized. Initilize should only be called once per game session.");
            return;
        }
        _isInitialized = true;
        GD.PrintRich("[color=#00ff88]SaveManager initialized.[/color]");
    }
}