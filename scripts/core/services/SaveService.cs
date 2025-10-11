namespace Core;

using Godot;
using Core.Interface;
/// <summary>
/// Manages user save data.
/// </summary>
public sealed class SaveService : ISaveService
{
    private bool _isInitialized;
    public SaveService()
    {
        _isInitialized = false;
        Initilize();
    }
    private void Initilize()
    {
        if (_isInitialized)
        {
            GD.PrintErr("SaveService is already initialized. Initilize should only be called once per game session.");
            return;
        }
        _isInitialized = true;
        GD.PrintRich("[color=#00ff88]SaveService initialized.[/color]");
    }
}