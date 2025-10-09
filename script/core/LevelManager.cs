namespace Core;
using Core.Interface;
using Godot;
public sealed partial class LevelManager : ILevelManager
{
    public PackedScene CurrentLevel { get; private set; }
    public Node2D LevelInstance { get; private set; }
    public Node ParentNode { get; private set; }
    public string LevelName { get; private set; }
    private bool _isInitialized = false;
    public LevelManager()
    {
        _isInitialized = false;
        Initilize();
    }
    /// <summary>
    /// Loads a level from a PackedScene and adds it to the specified parent node.
    /// </summary>
    /// <param name="levelScene">The PackedScene of the level to load.</param>
    /// <param name="parentNode">The parent node to which the level instance will be added. If null, uses the existing ParentNode.</param>
    /// <remarks>
    /// If ParentNode is already set and a different parentNode is provided, an error is logged and the method returns without making changes.
    /// If LevelInstance already exists, it is freed before loading the new level.
    /// </remarks>
    public void LoadLevel(PackedScene levelScene, Node parentNode = null)
    {
        if (!_isInitialized)
        {
            GD.PrintErr("LevelManager is not initialized. Call Initilize before loading levels.");
            return;
        }
        if (parentNode != null
            && ParentNode != null
            && parentNode != ParentNode)
        {
            GD.PrintErr("LevelManager: ParentNode is already set to a different node. Cannot change ParentNode after it has been set.");
            return;
        }
        if (parentNode != null)
        {
            ParentNode = parentNode;
        }
        if (levelScene == null)
        {
            GD.PrintErr("LevelManager: LoadLevel called with null levelScene.");
            return;
        }
        CurrentLevel = levelScene;
        if (LevelInstance != null)
        {
            LevelInstance.QueueFree();
        }
        LevelInstance = CurrentLevel.Instantiate<Node2D>();
        if (ParentNode != null)
        {
            ParentNode.AddChild(LevelInstance);
        }
        LevelName = LevelInstance.Name;
        GD.Print($"LevelManager: Loaded level '{LevelName}'.");
    }
    public void UnloadLevel()
    {
        if (LevelInstance != null)
        {
            LevelInstance.QueueFree();
            LevelInstance = null;
            CurrentLevel = null;
            LevelName = null;
            GD.Print("LevelManager: Unloaded current level.");
        }
        else
        {
            GD.PrintErr("LevelManager: No level is currently loaded to unload.");
        }
    }
    private void Initilize()
    {
        if (_isInitialized)
        {
            GD.PrintErr("LevelManager is already initialized. Initilize should only be called once per game session.");
            return;
        }
        _isInitialized = true;
        GD.PrintRich("[color=#00ff88]LevelManager initialized.[/color]");
    }
}