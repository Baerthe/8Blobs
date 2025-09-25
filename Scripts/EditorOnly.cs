using Godot;
using System;
/// <summary>
/// This script is used to mark nodes that should only exist in the editor. It will automatically delete itself when the game is running.
/// </summary>
public partial class EditorOnly : Node
{
	public override void _Ready() => QueueFree();
	public override void _Process(double delta) => QueueFree();
}
