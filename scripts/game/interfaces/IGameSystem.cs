namespace Game.Interface;

using Entities;
public interface IGameSystem
{
    bool IsInitialized { get; }
    public void Init();
    public void Update();
}