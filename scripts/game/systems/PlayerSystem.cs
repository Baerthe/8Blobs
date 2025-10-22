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
            OnLevelLoad(args.Templates, args.LevelInstance, args.PlayerInstance);
        };
    }
    public void OnLevelLoad(EntityIndex _, LevelEntity levelInstance, HeroEntity playerInstance)
    {
        if (IsInitialized) return;
        LevelInstance = levelInstance;
        LoadPlayer(_currentHeroData);
        PlayerInstance.SetProcess(true);
        PlayerInstance.SetPhysicsProcess(true);
        PlayerInstance.Show();
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
        		var velocity = Vector2.Zero;
		//What direction is the player going?
		if (Input.IsActionPressed("move_up"))
		{
			velocity.Y -= 1;
			PlayerInstance.CurrentDirection = PlayerDirection.Up;
		}
		if (Input.IsActionPressed("move_down"))
		{
			velocity.Y += 1;
			PlayerInstance.CurrentDirection = PlayerDirection.Down;
		}
		if (Input.IsActionPressed("move_left"))
		{
			velocity.X -= 1;
			PlayerInstance.CurrentDirection = PlayerDirection.Left;
		}
		if (Input.IsActionPressed("move_right"))
		{
			velocity.X += 1;
			PlayerInstance.CurrentDirection = PlayerDirection.Right;
		}
		if (velocity.Y != 0 && velocity.X != 0)
			PlayerInstance.CurrentDirection = PlayerDirection.Diagonal;
		if (velocity.Length() > 0)
		{
			velocity = velocity.Normalized() * (PlayerInstance.Data as HeroData).Stats.Speed;
			PlayerInstance.Sprite.Play();
		}
		else
			PlayerInstance.Sprite.Stop();
		// Move the player.
		Position += velocity * (float)delta;
		PlayerInstance.Sprite.FlipV = false; // Make sure we never flip vertically
		PlayerInstance.Sprite.FlipH = velocity.X < 0;
		PlayerInstance.MoveAndSlide();
		//ExecuteWeaponAttacks();
    }
    public void Update()
    {
    }
    public override void _ExitTree()
    {
        PlayerInstance.QueueFree();
        base._ExitTree();
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
        PlayerInstance = ResourceLoader.Load<PackedScene>(hero.Entity.ResourcePath).Instantiate<HeroEntity>();
        PlayerInstance.InitializeEntity(hero);
        PlayerInstance.Position = LevelInstance.PlayerSpawn.Position;
        PlayerInstance.SetProcess(false);
        PlayerInstance.SetPhysicsProcess(false);
        PlayerInstance.Hide();
        AddChild(PlayerInstance);
        GD.Print($"PlayerSystem: Loaded player '{hero.Name}'.");
    }
}
