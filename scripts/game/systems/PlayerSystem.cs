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
    public HeroEntity PlayerInstance { get; set; }
    private PackedScene _playerScene = null;
    public override void _Ready()
    {
        GD.Print("MobSystem Present.");
        GetParent<GameManager>().OnLevelLoad += (sender, args) =>
        {
            OnLevelLoad(args.PlayerInstance);
        };
    }
    public void OnLevelLoad(HeroEntity _)
    {
        if (IsInitialized) return;
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
    public void LoadPlayer(HeroData hero)
    {
        if (hero == null)
        {
            GD.PrintErr("PlayerSystem: LoadPlayer called with null hero data.");
            return;
        }
        if (PlayerInstance != null)
        {
            PlayerInstance.QueueFree();
        }
        _playerScene = hero.HeroEntityScene;
        PlayerInstance = ResourceLoader.Load<PackedScene>(_playerScene.ResourcePath).Instantiate<HeroEntity>();
        AddChild(PlayerInstance);
        GD.Print($"PlayerSystem: Loaded player '{hero.HeroName}'.");
    }
    public void Update()
    {
    }
    public override void _ExitTree()
    {
        PlayerInstance.QueueFree();
        base._ExitTree();
    }
}
