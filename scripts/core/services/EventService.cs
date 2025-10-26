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
    public EventService()
    {
        GD.PrintRich("[color=#00ff88]EventService initialized.[/color]");
    }
    public void UnsubscribeAll()
    {
        _typedSubs.Clear();
        _namedSubs.Clear();
        GD.PrintRich("[color=#ff8800]EventService: All subscriptions cleared. They will be recreated on next call.[/color]");
    }
    public void Subscribe<T>(Action<IEvent> handler)
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
    public void Subscribe(Action handler)
    {
        if (!_namedSubs.ContainsKey(handler.Method.Name))
        {
            GD.PrintRich($"[color=#0088ff]EventService: Creating subscription list for event name {handler.Method.Name}.[/color]");
            _namedSubs[handler.Method.Name] = new List<Action>();
        }
        GD.PrintRich($"[color=#0044FF]EventService: Subscribing handler to event name {handler.Method.Name}.[/color]");
        _namedSubs[handler.Method.Name].Add(handler);
    }
    public void Unsubscribe<T>(Action<IEvent> eventHandler)
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
    public void Unsubscribe(Action handler)
    {
        var name = handler.Method.Name;
        if (!_namedSubs.ContainsKey(name))
        {
            GD.PrintErr($"EventService: Attempted to unsubscribe from event name {name} but no subscriptions exist. Misspelled? Already unsubscribed?");
            return;
        }
        foreach (var key in _namedSubs.Keys)
        {
            _namedSubs[key].Remove(handler);
        }
        if (_namedSubs[name].Count == 0)
        {
            _namedSubs.Remove(name);
        }
    }
    public void Publish<T>(IEvent eventData)
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
                ((Action<IEvent>)handler)(eventData);
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