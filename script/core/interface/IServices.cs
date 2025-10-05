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
    public CoreContainer CoreContainer { get; }
    public ToolContainer ToolContainer { get; }
}