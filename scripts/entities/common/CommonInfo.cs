namespace Entities;

using Godot;
/// <summary>
/// CommonInfo is a Resource that holds common information attributes shared across various entities in the game.
/// </summary>
[GlobalClass]
public partial class CommonInfo : Resource
{
    [Export] public string Name { get; private set; } = "";
    [Export] public string Description { get; private set; } = "";
    [Export] public string Lore { get; private set; } = "";
    [Export] public bool Unlocked { get; private set; } = false;
    public void Unlock() => Unlocked = true;
}