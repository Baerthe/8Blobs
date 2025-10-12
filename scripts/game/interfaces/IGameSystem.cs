namespace Game.Interface;

using Godot;
public interface IGameSystem
{
    bool IsInitialized { get; }
    public void Update();
}