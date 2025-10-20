namespace Game.Interface;

using Godot;
using Entities;
public interface IGameSystem
{
    LevelEntity LevelInstance { get; set; }
    HeroEntity PlayerInstance { get; set; }
    bool IsInitialized { get; }
    public void Update();
    public void OnLevelLoad(LevelEntity levelInstance, HeroEntity playerInstance);
}