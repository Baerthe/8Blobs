namespace Core;
using Godot;
using Core.Interface;
public sealed class LoadService : ILoadService
{
    private bool _isInitialized;
    public LoadService()
    {
        _isInitialized = false;
        Initilize();
    }
    private void Initilize()
    {
        if (_isInitialized)
        {
            GD.PrintErr("LoadService is already initialized. Initilize should only be called once per game session.");
            return;
        }
        _isInitialized = true;
        GD.PrintRich("[color=#00ff88]LoadService initialized.[/color]");
    }
}