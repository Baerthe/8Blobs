namespace Tool.Interface;

using Godot;
/// <summary>
/// A tool for handling level related functionalities. It manages references to loaded level elements and processes level-specific logic.
/// </summary>
public interface ILevelTool : ITool
{
    Player player { get; }
    Camera2D camera { get; }
    Vector2 OffsetBetweenPickupAndPlayer { get; }
    Vector2 OffsetBetweenMobAndPlayer { get; }

}