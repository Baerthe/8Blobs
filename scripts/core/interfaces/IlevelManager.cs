namespace Core.Interface;
using Godot;
/// <summary>
/// Interface for the LevelManager; this manager handles level transitions and loading.
/// </summary>
public interface ILevelManager : ICore
{
    PackedScene CurrentLevel { get; }
    Node2D LevelInstance { get; }
    Node ParentNode { get; }
    void LoadLevel(PackedScene levelScene, Node parentNode = null);
    void UnloadLevel();
}