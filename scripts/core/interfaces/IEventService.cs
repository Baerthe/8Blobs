namespace Core.Interface;

using System;
/// <summary>
/// Interface for the EventService which manages event publishing and subscribing within the game.
/// </summary>
public interface IEventService
{
    /// <summary>
    /// Subscribes to an event of type IEvent.
    /// </summary>
    /// <typeparam name="IEvent"></typeparam>
    /// <param name="handler"></param>
    void Subscribe<T>(Action<IEvent> handler);
    /// <summary>
    /// Subscribes to a general event without data.
    /// </summary>
    /// <param name="handler"></param>
    void Subscribe(Action handler);
    /// <summary>
    /// Unsubscribes from an event of type IEvent.
    /// </summary>
    /// <typeparam name="IEvent"></typeparam>
    /// <param name="handler"></param>
    void Unsubscribe<T>(Action<IEvent> handler);
    /// <summary>
    /// Unsubscribes from a general event without data.
    /// </summary>
    void Unsubscribe(Action handler);
    /// <summary>
    /// Unsubscribes all event handlers from all events.
    /// </summary>
    void UnsubscribeAll();
    /// <summary>
    /// Publishes an event of type T to all subscribed handlers.
    /// </summary>
    /// <typeparam name="IEvent"></typeparam>
    /// <param name="eventData"></param>
    void Publish<T>(IEvent eventData);
    /// <summary>
    /// Publishes a general event without data.
    /// </summary>
    /// <param name="eventName"></param>
    void Publish(string eventName);
}