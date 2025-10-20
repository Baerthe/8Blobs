namespace Entities;

using Godot;
using System;
/// <summary>
/// The Entity class for Items, stores components and runtime data
/// </summary>
[GlobalClass]
public partial class ItemEntity : Node2D
{
    public int CurrentStackSize { get; set; } = 1;
    public override void _Ready()
    {
        AddToGroup("items");
    }
}