namespace Core.Interface;

using Entities;
using Godot;
/// <summary>
/// Interface for the HeroService; this Service handles hero data and management.
/// </summary>
public interface IHeroService
{
    HeroData CurrentHero { get; }
    Node ParentNode { get; }
    void LoadHero(HeroData heroData);
    void UnloadHero();
}