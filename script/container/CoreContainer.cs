namespace Container;
using Godot;
using System;
using System.Collections.Generic;
using Container.Interface;
/// <summary>
/// A container for core services, the managers; these are singletons that persist through scenes.
/// </summary>
public partial class CoreContainer : Node, IContainer
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
        return tool as T;
    }
}