namespace Tool;

using Godot;
using Tool.Interface;
public partial class LevelTool : Node2D, ILevelTool
{
    [Export] public Player player { get; set; }
    [Export] public Camera2D camera { get; set; }
}