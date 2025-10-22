namespace Game.Interface;

using Entities;
public interface IGameSystem
{
    HeroEntity PlayerInstance { get; }
    bool IsInitialized { get; }
    public void Update();
    public void OnLevelLoad(EntityIndex templates, LevelEntity levelInstance, HeroEntity playerInstance);
}