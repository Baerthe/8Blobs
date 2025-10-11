namespace Game;

using Godot;
//TODO: This is setup as more so a node, we need a manager, create chest res
public partial class ChestSystem : Area2D
{
    [Export] internal ChestState State { get; set; } = ChestState.Locked;
    [Export] public string Contains { get; set; } = "gold_001";
    [Export] public int Amount { get; set; } = 10;
    private AnimatedSprite2D _sprite2D;
    private CollisionShape2D _collision2D;
    private bool _isOpened = false;
    public override void _Ready()
    {
        _sprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _collision2D = GetNode<CollisionShape2D>("CollisionShape2D");
        if (_sprite2D == null) GD.PrintErr("Chest: Sprite2D is null.");
        if (_collision2D == null) GD.PrintErr("Chest: Collision2D is null.");
        _sprite2D.Animation = "Closed";
        Connect("body_entered", new Callable(this, nameof(OnBodyEntered)));
    }
    private void OnBodyEntered(Node body)
    {
        if (_isOpened) return;
        if (body is Player player)
        {
            OpenChest(player);
        }
    }
    private void OpenChest(Player player)
    {
        if (State == ChestState.Locked)
        {
            GD.PrintRich("[color=#ff0000]Chest is locked! Cannot open.[/color]");
            return;
        }
        // TODO: Add in buy logic for locked
        _isOpened = true;
        _sprite2D.Animation = "Open";
        _collision2D.Disabled = true;
        // Give the player the contents of the chest.
        //TODO: Implement inventory system and add item to player inventory.
        GD.PrintRich($"[color=#00ff88]Player {player.Name} opened chest and received {Amount} of {Contains}.[/color]");
    }
    internal enum ChestState
    {
        Locked,
        Opened
    }
}