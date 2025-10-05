namespace Container;
using Godot;
using System;
using System.Collections.Generic;
/// <summary>
/// A container for tools; these are node based services that can be added to scenes as needed.
/// </summary>
public partial class ToolContainer : Node
{
    private Dictionary<Type, Node> _tools = new();
    /// <summary>
    /// Registers a tool node with the container.
    /// </summary>
    /// <typeparam name="Tinterface">The interface type of the tool.</typeparam>
    /// <typeparam name="TImplementation">The implementation type of the tool.</typeparam>
    public void Register<Tinterface, TImplementation>() where TImplementation : Tinterface, new()
    {
        _tools[typeof(Tinterface)] = new TImplementation() as Node;
        GD.Print($"Registered tool: {typeof(Tinterface).Name} as {typeof(TImplementation).Name}");
    }
    /// <summary>
    /// Resolves a tool node from the container. If the tool is not already in the scene tree, it duplicates it and adds it to the scene tree.
    /// If the tool is already in the scene tree, it returns null.
    /// </summary>
    /// <typeparam name="T">The type of the tool node to resolve.</typeparam>
    /// <returns>The resolved tool node, or null if it's already in the scene tree.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public T Resolve<T>() where T : Node
    {
        _tools.TryGetValue(typeof(T), out var tool);
        if (tool == null)
        {
            GD.PrintErr($"Tool not found: {typeof(T).Name}");
            throw new InvalidOperationException($"ERROR 099: Tool node not found: {typeof(T).Name} in Tools. Level cannot load.");
        }
        GD.Print($"Delivery Time! Resolving tool: {typeof(T).Name} as {tool?.GetType().Name ?? "null"}");
        if (tool != null && tool.GetParent() == null)
        {
            GD.Print($"Tool Delievered to Scene: {typeof(T).Name} as {tool?.GetType().Name ?? "null"}. Copying to Scene Tree");
            var child = tool.Duplicate();
            if (child == null)
            {
                GD.PrintErr($"Failed to duplicate tool: {typeof(T).Name} as {tool?.GetType().Name ?? "null"}");
                return null;
            }
            AddChild(child);
            return child as T;
        }
        GD.PrintErr($"Tool {typeof(T).Name} as {tool?.GetType().Name ?? "null"} already in Scene Tree or not found.");
        return null;
    }
}