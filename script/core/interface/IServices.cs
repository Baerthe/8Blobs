namespace Core.Interface;
using Godot;
using Container;
using System;
using System.Collections.Generic;
/// <summary>
/// A service locator interface for managing core and tool singletons. This is used for dependency injection.
/// </summary>
public interface IServices
{
    /// <summary>
    /// The core container for managing core singletons. Cores are not nodes; but global helpers.
    /// </summary>
    public CoreContainer CoreContainer { get; }
    /// <summary>
    /// The tool container for managing tool nodes. Tools get copied into the scene.
    /// </summary>
    public ToolContainer ToolContainer { get; }
    /// <summary>
    /// We build the Tools and add them after the Cores because the Tools may require a Core(s).
    /// </summary>
    public void DelayedToolBuilder();
}