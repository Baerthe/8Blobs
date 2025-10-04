namespace Core.Interface;
using Godot;
/// <summary>
/// Interface for the LevelManager; this manager handles level transitions and loading.
/// </summary>
public interface ILevelManager
{
    /// <summary>
    /// The current level's PackedScene.
    /// </summary>
    public PackedScene CurrentLevel { get; }
    /// <summary>
    /// The current level's instance as a Node2D.
    /// </summary>
    public Node2D LevelInstance { get; }
}