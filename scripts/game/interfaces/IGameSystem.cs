namespace Game.Interface;

using Godot;
using Entities;
public interface IGameSystem
{
    HeroEntity PlayerInstance { get; }
    bool IsInitialized { get; }
    public void Update();
    public void OnLevelLoad(LevelEntity levelInstance, HeroEntity playerInstance);
}