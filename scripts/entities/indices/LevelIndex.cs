namespace Entities;

using Godot;
/// <summary>
/// Holds references to level scenes for easy access.
/// </summary>
[GlobalClass]
public sealed partial class LevelIndex : Resource
{
    [ExportCategory("Level Scenes")]
    [Export] public PackedScene[] LevelScenes { get; private set; }
}