namespace Entities;

using Godot;
/// <summary>
/// Holds references to hero scenes for easy access.
/// </summary>
[GlobalClass]
public sealed partial class HeroIndex : Resource
{
    [ExportCategory("Hero Scenes")]
    [Export] public PackedScene[] HeroScenes { get; private set; }
}