namespace Core;
using Core.Interface;
using Tool.Interface;
using Container;
using Godot;

/// <summary>
/// Where the magic happens; builds our dependency injection containers for core and tool singletons.
/// </summary>
public sealed class Services : IServices
{
    public CoreContainer CoreContainer { get; } = new();
    public ToolContainer ToolContainer { get; } = new();
    public Services()
    {
        BuildCoreContainer();
    }
    public void DelayedToolBuilder()
    {
        if (ToolContainer != null) return;
        BuildToolContainer();
    }
    /// <summary>
    /// Builds the core container with all core singletons.
    /// </summary>
    /// <remarks>
    /// The core container contains singletons that are essential for the application's core functionality.
    /// </remarks>
    private void BuildCoreContainer()
    {
        GD.PrintRich("[color=#00ff00]Building Core Container...[/color]");
        CoreContainer.Register<IClockManager, ClockManager>();
        CoreContainer.Register<IPlayerDataManager, PlayerDataManager>();
        CoreContainer.Register<ILevelManager, LevelManager>();
        GD.PrintRich("[color=#00ff00]Core Container Built.[/color]");
    }
    /// <summary>
    /// Builds the tool container with all tool singletons.
    /// </summary>
    /// <remarks>
    /// The tool container contains singletons that are nodes; these get copied into a level's scene tree when the level is loaded.
    /// This way, we don't need to worry about adding these manually, setting them up, and clearing them; its handled for us.
    /// </remarks>
    private void BuildToolContainer()
    {
        GD.PrintRich("[color=#0088ff]Building Tool Container...[/color]");
        ToolContainer.Register<ITilingTool, TilingTool>();
        GD.PrintRich("[color=#0088ff]Tool Container Built.[/color]");
    }
}