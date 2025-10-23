namespace Core;

using Godot;
using System;
using Core.Interface;
using System.Collections.Generic;
/// <summary>
/// Service that manages event publishing and subscribing within the game. A central hub for event-driven communication.
/// Publish events to the service so that other parts of the game, like systems, can subscribe to them. It supports both custom typed events (say you need to send some data) and named events (just a simple signal with no data).
/// </summary>
/// <remarks>
/// The EventService allows different parts of the game to communicate without direct references, promoting loose coupling through the singleton core service provider.
/// Sadly, no likes, only subs UwU (and pubs lol).
/// </remarks>
public sealed class EventService : IEventService
{
    private Dictionary<Type, List<Delegate>> _typedSubs = new();
    private Dictionary<string, List<Action>> _namedSubs = new();
    private bool _isInitialized = false;
    public EventService()
    {
        _isInitialized = false;
        Initialize();
    }
    private void Initialize()
    {
        if (_isInitialized)
        {
            GD.PrintErr("EventService already initialized!");
            return;
        }
        _isInitialized = true;
        GD.PrintRich("[color=#00ff88]EventService initialized.[/color]");
    }
    public void Subscribe<T>(Action<T> handler) where T : class
    {

    }
    public void Subscribe(string eventName, Action handler)
    {

    }
    public void Unsubscribe<T>(Action<T> eventHandler) where T : class
    {

    }
    public void Publish<T>(T eventData) where T : class
    {

    }
}