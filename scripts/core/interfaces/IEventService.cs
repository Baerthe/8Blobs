namespace Core.Interface;

using System;
/// <summary>
/// Interface for the EventService which manages event publishing and subscribing within the game.
/// </summary>
public interface IEventService
{
    /// <summary>
    /// Subscribes to an event of type T.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="handler"></param>
    void Subscribe<T>(Action<T> handler) where T : class;
    /// <summary>
    /// Subscribes to a general event without data.
    /// </summary>
    /// <param name="handler"></param>
    void Subscribe(Action handler);
    /// <summary>
    /// Unsubscribes from an event of type T.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="handler"></param>
    void Unsubscribe<T>(Action<T> handler) where T : class;
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
    /// <typeparam name="T"></typeparam>
    /// <param name="eventData"></param>
    void Publish<T>(T eventData) where T : class;
    /// <summary>
    /// Publishes a general event without data.
    /// </summary>
    /// <param name="eventName"></param>
    void Publish(string eventName);
}