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
    public LevelEntity LevelInstance { get; private set; }
    private HeroData _currentHeroData = null;
    private PackedScene _playerScene = null;
    public override void _Ready()
    {
        GD.Print("PlayerSystem Present.");
        GetParent<GameManager>().OnLevelLoad += (sender, args) =>
        {
            OnLevelLoad(args.LevelInstance, args.PlayerInstance);
        };
        _currentHeroData = Core.CoreProvider.GetHeroService().CurrentHero;
        LoadPlayer(_currentHeroData);
    }
    public void OnLevelLoad(LevelEntity levelInstance, HeroEntity __)
    {
        if (IsInitialized) return;
        LevelInstance = levelInstance;
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
    public void Update()
    {
    }
    public override void _ExitTree()
    {
        PlayerInstance.QueueFree();
        base._ExitTree();
    }
    private void LoadPlayer(HeroData hero)
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
        PlayerInstance = ResourceLoader.Load<PackedScene>(hero.Entity.ResourcePath).Instantiate<HeroEntity>();
        PlayerInstance.InitializeEntity(hero);
        PlayerInstance.Position = LevelInstance.PlayerSpawn.Position;
        PlayerInstance.SetProcess(false);
        PlayerInstance.SetPhysicsProcess(false);
        PlayerInstance.Hide();
        AddChild(PlayerInstance);
        GD.Print($"PlayerSystem: Loaded player '{hero.HeroName}'.");
    }
}
