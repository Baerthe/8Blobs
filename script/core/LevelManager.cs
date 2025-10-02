namespace Core;
using Core.Interface;
using Godot;
/// <summary>
/// A manager for handling level transitions and loading.
/// </summary>
public sealed partial class LevelManager : Node, ILevelManager
{
    private PackedScene _currentLevel;
    private Node2D _levelInstance;
    private readonly Node _parentNode;

    public LevelManager(Node parentNode)
    {
        _parentNode = parentNode;
    }
}