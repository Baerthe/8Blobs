namespace Core;

using Core.Interface;
/// Non-data events for Game Systems.
public sealed class Init : IEvent;
// Player Events
public sealed class PlayerSpawn : IEvent;
public sealed class PlayerDefeat : IEvent;