namespace Core.Interface;

using Godot;
/// <summary>
/// Interface for TilingManager to handle tiling of scene map elements.
/// </summary>
public interface ITilingManager
{
    void LoadTiles();
    void PlayerCrossedBorder(Player player);
}
