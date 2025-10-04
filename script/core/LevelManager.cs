namespace Core;
using Core.Interface;
using Godot;
public sealed partial class LevelManager : ILevelManager
{
    private PackedScene _currentLevel;
    private Node2D _levelInstance;
    private readonly Node _parentNode;

    public LevelManager(Node parentNode)
    {
        _parentNode = parentNode;
    }
}