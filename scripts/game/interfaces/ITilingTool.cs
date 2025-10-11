namespace Tool.Interface;
/// <summary>
/// Interface for TilingManager to handle tiling of scene map elements.
/// </summary>
public interface ITilingTool : ITool
{
    void LoadTiles();
    void PlayerCrossedBorder(Player player);
}
