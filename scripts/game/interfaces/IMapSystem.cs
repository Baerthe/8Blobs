namespace Game.Interface;
/// <summary>
/// Interface for the map system to handle tiling of scene map elements.
/// </summary>
public interface IMapSystem
{
    void LoadTiles();
    void PlayerCrossedBorder(Player player);
}
