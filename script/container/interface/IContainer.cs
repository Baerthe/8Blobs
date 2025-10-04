namespace Container.Interface;
using System;
using Godot;
/// <summary>
/// Interface for Containers; these are a simple key/value store for managers (singletons) and tools (node services) by a simple registration and resolution mechanism.
/// </summary>
public interface IContainer
{
    void Register<Tinterface, TImplementation>() where TImplementation : Tinterface, new();
    T Resolve<T>() where T : class;
}