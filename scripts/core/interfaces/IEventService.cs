namespace Core.Interface;

using Godot;
using System;
public interface IEventService
{
    void Subscribe<T>(Action<T> handler) where T : class;
    void Unsubscribe<T>(Action<T> handler) where T : class;
    void Publish<T>(T eventData) where T : class;
}