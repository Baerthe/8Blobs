namespace Container;
using Godot;
using System;
using System.Collections.Generic;
using Container.Interface;
/// <summary>
/// A container for tools; these are node based services that can be added to scenes as needed.
/// </summary>
public partial class ToolContainer : Node, IContainer
{
    private Dictionary<Type, Node> _tools = new();
    public void Register<Tinterface, TImplementation>() where TImplementation : Tinterface, new()
    {
        _tools[typeof(Tinterface)] = new TImplementation() as Node;
        GD.Print($"Registered tool: {typeof(Tinterface).Name} as {typeof(TImplementation).Name}");
    }
    public T Resolve<T>() where T : class
    {
        _tools.TryGetValue(typeof(T), out var tool);
        GD.Print($"Delivery Time! Resolving tool: {typeof(T).Name} as {tool?.GetType().Name ?? "null"}");
        if (tool != null && tool.GetParent() == null)
        {
            GD.Print($"Tool Delievered to Scene: {typeof(T).Name} as {tool?.GetType().Name ?? "null"}. Copying to Scene Tree");
            var child = tool.Duplicate() as Node;
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