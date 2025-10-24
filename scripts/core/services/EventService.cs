namespace Core;

using Godot;
using System;
using Core.Interface;
using System.Collections.Generic;
/// <summary>
/// Service that manages event publishing and subscribing within the game; Publish events to the service so that other parts of the game, like systems, can subscribe to them.
/// It supports both custom typed events (say you need to send some data) and named events (just a simple signal with no data).
/// These are not Godot Signals; these are delegates being managed.
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
    public void UnsubscribeAll()
    {
        _typedSubs.Clear();
        _namedSubs.Clear();
        GD.PrintRich("[color=#ff8800]EventService: All subscriptions cleared. They will be recreated on next call.[/color]");
    }
    public void Subscribe<T>(Action<T> handler) where T : class
    {
        var type = typeof(T);
        if (!_typedSubs.ContainsKey(type))
        {
            GD.PrintRich($"[color=#0088ff]EventService: Creating subscription list for event type {type.Name}.[/color]");
            _typedSubs[type] = new List<Delegate>();
        }
        GD.PrintRich($"[color=#0044FF]EventService: Subscribing handler to event type {type.Name}.[/color]");
        _typedSubs[type].Add(handler);
    }
    public void Subscribe(string eventName, Action handler)
    {
        if (!_namedSubs.ContainsKey(eventName))
        {
            GD.PrintRich($"[color=#0088ff]EventService: Creating subscription list for event name {eventName}.[/color]");
            _namedSubs[eventName] = new List<Action>();
        }
        GD.PrintRich($"[color=#0044FF]EventService: Subscribing handler to event name {eventName}.[/color]");
        _namedSubs[eventName].Add(handler);
    }
    /// <summary>
    /// Unsubscribes a handler from events of type T.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="eventHandler"></param>
    public void Unsubscribe<T>(Action<T> eventHandler) where T : class
    {
        var type = typeof(T);
        if (_typedSubs.ContainsKey(type))
        {
            bool removed = _typedSubs[type].Remove(eventHandler);
            if (removed)
            {
                GD.PrintRich($"[color=#FF4444]EventService: Unsubscribed handler from event type {type.Name}.[/color]");
            }
            else
            {
                GD.PrintErr($"EventService: Attempted to unsubscribe handler from event type {type.Name} but handler was not found. Never subbed or already unsubscribed?");
            }
            if (_typedSubs[type].Count == 0)
            {
                _typedSubs.Remove(type);
            }
        }
    }
    public void Unsubscribe(string eventName, Action handler)
    {
        if (_namedSubs.ContainsKey(eventName))
        {
            _namedSubs[eventName].Remove(handler);
        }
        else
        {
            GD.PrintErr($"EventService: Attempted to unsubscribe from event name {eventName} but no subscriptions exist. Misspelled? Already unsubscribed?");
        }
        if (_namedSubs[eventName].Count == 0)
        {
            _namedSubs.Remove(eventName);
        }
    }
    /// <summary>
    /// Publishes an event of type T to all subscribed handlers. There are two options for eventData:
    /// 1. Define a custom class for the event data and use that as T.
    ///     - This is type-safe and allows for structured data.
    ///     - Example: CoreProvider.EventService().Publish(new ScoreEvent { Score = 100, Player = "John" });
    /// 2. Use the DataEvent class for dynamic event data which is a dictionary container.
    ///     - This is less type-safe but more flexible for ad-hoc events.
    ///     - Example: CoreProvider.EventService().Publish(new DataEvent { Data = new Dictionary<string, object> { { "Score", 100 }, { "Player", "John" } } });
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="eventData"></param>
    /// <remarks>
    /// If you need to send simple signals without data needing to be passed, use the string event name overload.
    ///     - Example: CoreProvider.EventService().Publish("PlayerDied");
    /// </remarks>
    public void Publish<T>(T eventData) where T : class
    {
        var type = typeof(T);
        if (!_typedSubs.ContainsKey(type))
        {
            GD.PrintErr($"EventService: Publish called for event type {type.Name} but no subscriptions exist. Did you forget to subscribe?");
            return;
        }
        else
        {
            foreach (var handler in _typedSubs[type])
            {
                ((Action<T>)handler)(eventData);
            }
        }
    }
    public void Publish(string eventName)
    {
        if (!_namedSubs.ContainsKey(eventName))
        {
            GD.PrintErr($"EventService: Publish called for event name {eventName} but no subscriptions exist. Did you forget to subscribe?");
            return;
        }
        else
        {
            foreach (var handler in _namedSubs[eventName])
            {
                handler.Invoke();
            }
        }
    }
}
/// <summary>
/// Generic data event class for dynamic event data. This is not type safe but allows for flexible event data structures.
/// This is mostly intended for events where the data structure is not known at compile time or for testing purposes.
/// You should almost always prefer defining a custom class for your event data for type safety and clarity.
/// </summary>
public class DataEvent
{
    public Dictionary<string, object> Data { get; set; } = new();
}