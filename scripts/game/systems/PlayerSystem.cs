namespace Game;

using Godot;
using Core;
using Entities;
using Game.Interface;
using System.Collections.Generic;

/// <summary>
/// The player is the main character that the user controls. This class handles movement, health, and collisions with mobs.
/// </summary>
public sealed partial class PlayerSystem : Node2D, IGameSystem
{
    public bool IsInitialized { get; private set; } = false;
    public HeroEntity PlayerInstance { get; private set; }
    public LevelEntity LevelInstance { get; private set; }
    public List<ItemEntity> Items { get; private set; } = new();
    public List<WeaponEntity> Weapons { get; private set; } = new();
    private PackedScene _playerScene = null;
    public override void _Ready()
    {
        GD.Print("PlayerSystem Present.");
        GetParent<GameManager>().OnLevelLoad += (sender, args) => OnLevelLoad(args.Templates, args.LevelInstance, args.PlayerInstance);
        LoadPlayer(CoreProvider.HeroService().CurrentHero);
    }
    public override void _Process(double delta)
    {
        if (!IsInitialized) return;
        if (PlayerInstance == null) return;
        if (PlayerInstance.CurrentHealth <= 0)
        {
            GD.Print("Player has died.");
            //TODO: Defeat();
        }
        switch (PlayerInstance.CurrentDirection)
        {
            case PlayerDirection.Up:
                PlayerInstance.Sprite.Animation = "Up";
                break;
            case PlayerDirection.Down:
                PlayerInstance.Sprite.Animation = "Down";
                break;
            case PlayerDirection.Diagonal:
                PlayerInstance.Sprite.Animation = "Right";
                break;
            case PlayerDirection.Left:
                PlayerInstance.Sprite.FlipH = true;
                PlayerInstance.Sprite.Animation = "Right";
                break;
            case PlayerDirection.Right:
                PlayerInstance.Sprite.FlipH = false;
                PlayerInstance.Sprite.Animation = "Right";
                break;
            default:
                PlayerInstance.Sprite.Animation = "Idle";
                break;
        }
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
    public override void _ExitTree()
    {
        PlayerInstance.QueueFree();
        GetParent<GameManager>().OnLevelLoad -= (sender, args) => OnLevelLoad(args.Templates, args.LevelInstance, args.PlayerInstance);
    }
    public void OnLevelLoad(EntityIndex _, LevelEntity levelInstance, HeroEntity __)
    {
        if (IsInitialized) return;
        LevelInstance = levelInstance;
        PlayerInstance.Show();
        IsInitialized = true;
    }
    public void Update()
    {
    }
    /// <summary>
    /// Adds an item to the player's inventory. If the item already exists and is stackable, it increases the stack size.
    /// </summary>
    /// <param name="item"></param>
    public void AddItem(ItemEntity item)
    {
        if (item == Items.Find(i => i.Data.Name == item.Data.Name))
        {
            item.QueueFree();
            item = Items[Items.IndexOf(item)];
            var itemData = item.Data as ItemData;
            item.CurrentStackSize += 1;
            if (item.CurrentStackSize > itemData.MaxStackSize)
                item.CurrentStackSize = itemData.MaxStackSize;
            return;
        }
        Items.Add(item);
    }
    public void AddWeapon(WeaponEntity weapon)
    {
        if (weapon == Weapons.Find(w => w.Data.Name == weapon.Data.Name))
            return;
        Weapons.Add(weapon);
    }
    private void Defeat()
    {
        GD.Print("PlayerSystem: Defeat sequence triggered.");
        IsInitialized = false;
        PlayerInstance.Hide();
        Items.Clear();
        Weapons.Clear();
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
        PlayerInstance.Inject(hero);
        PlayerInstance.Position = LevelInstance.PlayerSpawn.Position;
        PlayerInstance.CurrentHealth = hero.Stats.Health;
        PlayerInstance.Hide();
        AddChild(PlayerInstance);
        GD.Print($"PlayerSystem: Loaded player '{hero.Name}'.");
    }
}
