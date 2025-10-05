namespace Core;
using Container;
using Core.Interface;
using Godot;
using System;
using System.Collections.Generic;
/// <summary>
/// Where the magic happens; builds our dependency injection containers for core and tool singletons.
/// </summary>
public sealed class Services : IServices
{
    /// <summary>
    /// The core container for managing core singletons. Cores are not nodes; but global helpers.
    /// </summary>
    public CoreContainer CoreContainer { get; } = new();
    /// <summary>
    /// The tool container for managing tool nodes. Tools get copied into the scene.
    /// </summary>
    public ToolContainer ToolContainer { get; } = new();
    /// <summary>
    /// Constructor for the Services class. Initializes the core and tool containers.
    /// </summary>
    public Services()
    {
        BuildCoreContainer();
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
        CoreContainer.Register<IClockManager, ClockManager>();
        CoreContainer.Register<ILevelManager, LevelManager>();
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
    }
}