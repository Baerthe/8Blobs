namespace Entities.Interfaces;

using Godot;
/// <summary>
/// Interface for data resources
/// </summary>
public interface IData
{
    public string Name { get; }
    public string Description { get; }
    public string Lore { get; }
}