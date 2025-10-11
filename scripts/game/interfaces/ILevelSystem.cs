namespace Game.Interface;
using Godot;
/// <summary>
/// A tool for handling level related functionalities. It manages references to loaded level elements and processes level-specific logic.
/// </summary>
public interface ILevelSystem
{
    Player player { get; }
    Camera2D camera { get; }
    Vector2 OffsetBetweenPickupAndPlayer { get; }
    Vector2 OffsetBetweenMobAndPlayer { get; }
    Path2D MobPath { get; set; }
    PathFollow2D MobSpawner { get; set; }
    Path2D PickupPath { get; set; }
    PathFollow2D PickupSpawner { get; set; }

}