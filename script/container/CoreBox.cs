namespace Container;
using Godot;
using Core;
using Core.Interface;
/// <summary>
/// Where the magic happens; builds our dependency injection containers for core and tool singletons.
/// </summary>
public static class CoreBox
{
    private static CoreContainer CoreContainer { get; } = new();
    private static bool _isBuilt = false;
    static CoreBox()
    {
        InitilizationCheck();
    }
    public static IClockManager GetClockManager()
    {
        return CoreContainer.Resolve<IClockManager>();
    }
    public static IPlayerDataManager GetPlayerDataManager()
    {
        return CoreContainer.Resolve<IPlayerDataManager>();
    }
    private static void InitilizationCheck()
    {
        if (!_isBuilt)
            BuildCoreContainer();
        ToolBox.BuildToolContainer();
    }
    /// <summary>
    /// Builds the core container with all core singletons.
    /// </summary>
    /// <remarks>
    /// The core container contains singletons that are essential for the application's core functionality.
    /// </remarks>
    private static void BuildCoreContainer()
    {
        if (_isBuilt)
            return;
        GD.PrintRich("[color=#00ff00]Registering Cores to CoreBox...[/color]");
        CoreContainer.Register<ISaveManager, SaveManager>();
        CoreContainer.Register<IClockManager, ClockManager>();
        CoreContainer.Register<IPlayerDataManager, PlayerDataManager>();
        CoreContainer.Register<ILevelManager, LevelManager>();
        GD.PrintRich("[color=#00ff00]Cores Registered.[/color]");
    }
}