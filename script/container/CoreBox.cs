namespace Container;
using Godot;
using Core;
using System;
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
        BuildCoreContainer();
        ToolBox.BuildToolContainer();
    }
    public static IClockManager GetClockManager()
    {
        if (!_isBuilt)
            BuildCoreContainer();
        return CoreContainer.Resolve<IClockManager>();
    }
    public static IPlayerDataManager GetPlayerDataManager()
    {
        if (!_isBuilt)
            BuildCoreContainer();
        return CoreContainer.Resolve<IPlayerDataManager>();
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
        CoreContainer.Register<IClockManager, ClockManager>();
        CoreContainer.Register<IPlayerDataManager, PlayerDataManager>();
        CoreContainer.Register<ILevelManager, LevelManager>();
        GD.PrintRich("[color=#00ff00]Cores Registered.[/color]");
    }
}