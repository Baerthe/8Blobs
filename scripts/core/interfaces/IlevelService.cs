namespace Core.Interface;
using Godot;
/// <summary>
/// Interface for the LevelService; this Service handles level transitions and loading.
/// </summary>
public interface ILevelService
{
    PackedScene CurrentLevel { get; }
    Node2D LevelInstance { get; }
    Node ParentNode { get; }
    void LoadLevel(PackedScene levelScene, Node parentNode = null);
    void UnloadLevel();
}