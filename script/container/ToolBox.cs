namespace Container;
using Godot;
using Core;
using System;
using Tool.Interface;
/// <summary>
/// Where the magic happens; builds our dependency injection containers for core and tool singletons.
/// </summary>
public static class ToolBox
{
    private static ToolContainer ToolContainer { get; } = new();
    private static bool _isBuilt = false;
    public static ITilingTool GetTilingTool()
    {
        if (!_isBuilt)
            BuildToolContainer();
        return ToolContainer.Resolve<ITilingTool>();
    }
    /// <summary>
    /// Builds the tool container with all tool singletons.
    /// </summary>
    /// <remarks>
    /// The tool container contains singletons that are nodes; these get copied into a level's scene tree when the level is loaded.
    /// This way, we don't need to worry about adding these manually, setting them up, and clearing them; its handled for us.
    /// </remarks>
    public static void BuildToolContainer()
    {
        if (_isBuilt)
            return;
        GD.PrintRich("[color=#0088ff]Registering Tools to ToolBox...[/color]");
        ToolContainer.Register<ITilingTool, TilingTool>();
        GD.PrintRich("[color=#0088ff]Tools Registered.[/color]");
        _isBuilt = true;
    }
}