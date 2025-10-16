namespace Game;

using Godot;
using Entities;
using Game.Interface;
/// <summary>
/// The player is the main character that the user controls. This class handles movement, health, and collisions with mobs.
/// </summary>
public sealed partial class PlayerSystem : Node2D, IGameSystem
{
    public bool IsInitialized { get; private set; } = false;
    public HeroEntity PlayerInstance { get; private set; }
    private PackedScene _playerScene = null;
    public override void _Ready()
    {
        IsInitialized = true;
    }
    public override void _Process(double delta)
    {
        if (!IsInitialized) return;
        if (PlayerInstance == null) return;
    }
    public override void _PhysicsProcess(double delta)
    {
        if (!IsInitialized) return;
        if (PlayerInstance == null) return;
    }
    public void LoadPlayer(PackedScene playerScene)
    {
        if (playerScene == null)
        {
            GD.PrintErr("PlayerSystem: LoadPlayer called with null playerScene.");
            return;
        }
        if (PlayerInstance != null)
        {
            PlayerInstance.QueueFree();
        }
        _playerScene = playerScene;
        PlayerInstance = _playerScene.Instantiate<HeroEntity>();
        AddChild(PlayerInstance);
        GD.Print($"PlayerSystem: Loaded player '{PlayerInstance.HeroName}'.");
    }
	public void Update()
    {
    }
}
