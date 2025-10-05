namespace Container;
using Godot;
using System;
using System.Collections.Generic;
/// <summary>
/// A container for core services, the managers; these are singletons that persist through scenes.
/// </summary>
/// <remarks>
/// Core services are not nodes; they are global helpers that manage the state and behavior of the application.
/// These are the core dependencies that get injected.
/// </remarks>
public partial class CoreContainer
{
    private Dictionary<Type, object> _cores = new();
    /// <summary>
    /// Registers a core service with its interface and implementation.
    /// </summary>
    /// <typeparam name="Tinterface">The interface type of the service.</typeparam>
    /// <typeparam name="TImplementation">The implementation type of the service.</typeparam>
    public void Register<Tinterface, TImplementation>() where TImplementation : Tinterface, new()
    {
        _cores[typeof(Tinterface)] = new TImplementation();
        GD.Print($"Registered core: {typeof(Tinterface).Name} as {typeof(TImplementation).Name}");
    }
    /// <summary>
    /// Resolves a core service by its interface type.
    /// </summary>
    /// <typeparam name="T">The interface type of the service to resolve.</typeparam>
    /// <returns>The resolved service instance.</returns>
    public T Resolve<T>() where T : class
    {
        _cores.TryGetValue(typeof(T), out var core);
        if (core == null)
        {
            GD.PrintErr($"Service of type {typeof(T).Name} is not registered.");
            throw new InvalidOperationException($"ERROR 098: Service of type {typeof(T).Name} is not registered in Cores. Game cannot load.");
        }
        GD.Print($"Delivery Time! Resolving core: {typeof(T).Name} as {core?.GetType().Name ?? "null"}");
        return core as T;
    }
}