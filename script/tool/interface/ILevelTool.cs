namespace Tool.Interface;
using Godot;
public interface ILevelTool : ITool
{
    Player player { get; set; }
    Camera2D camera { get; set; }
}