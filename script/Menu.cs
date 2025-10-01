using Godot;
/// <summary>
/// The menu class handles the main menu UI and interactions.
/// </summary>
public partial class Menu : Control
{
	[Signal] public delegate void StartGameEventHandler();
	[Export] private Button _startButton;
	[Export] private Button _quitButton;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_startButton.Pressed += () => EmitSignal(SignalName.StartGame);
		_quitButton.Pressed += () => GetTree().Quit();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
