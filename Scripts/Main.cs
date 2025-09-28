using Godot;
/// <summary>
/// The main class that handles the game logic, including spawning mobs and managing the game state.
/// </summary>
public partial class Main : Node2D
{
	[ExportGroup("Singles")]
	[ExportSubgroup("Core")]
	[Export] public Menu Menu { get; private set; }
	[Export] public Player Player { get; private set; }
	[Export] public Marker2D PlayerStart { get; private set; }
	[Export] public Camera2D Camera { get; private set; }
	[Export] public Ui Ui { get; private set; }

	[ExportSubgroup("Timers")]
	[Export] private Timer _mobSpawnTimer;
	[Export] private Timer _scoreTimer;
	[Export] private Timer _startTimer;
	[ExportGroup("Spawnables")]
	[ExportSubgroup("Mobs")]
	[Export] public PackedScene[] MobScenes { get; private set; }
	[Export] private PathFollow2D _mobSpawner;
	private int _score = 0;
	private bool _isGameOver = false;
	public override void _Ready()
	{
		Camera.Position = Player.Position;
		Menu.Show();
	}
	public override void _Process(double delta)
	{
		Ui.Update(delta, Player.Health, _score, _isGameOver);
		if (_isGameOver)
		{
			GameOver();
		}
		else
		{
			Camera.Position = Player.Position;
		}
	}
	public void GameOver()
	{
		_mobSpawnTimer.Stop();
		_scoreTimer.Stop();
		if (Input.IsActionPressed("ui_accept"))
			OnMenuStartGame();
	}
	public void NewGame()
	{
		_isGameOver = false;
		Menu.Hide();
		Player.Start(PlayerStart.Position);
		Camera.Position = Player.Position;
		GetTree().CallGroup("Mobs", Node.MethodName.QueueFree);
		_score = 0;
		_startTimer.Start();
		Ui.NewGame(_startTimer.WaitTime);
	}
	private void OnMobTimerTimeout()
	{
		Mob mob = (Mob)MobScenes[GD.Randi() % MobScenes.Length].Instantiate();
		_mobSpawner.ProgressRatio = GD.Randf();
		float direction = _mobSpawner.Rotation + Mathf.Pi / 2;
		var velocity = new Vector2((float)GD.RandRange(150.0, 250.0), 0);
		var mobBaseSpeed = (mob.Speed + (int)(GD.RandRange(-1.0, 0.5) % mob.Speed)) * new Vector2(1, 0).Rotated(direction);
		mob.Position = _mobSpawner.Position;
		mob.LinearVelocity = velocity.Rotated(direction) + mobBaseSpeed;
		AddChild(mob);
	}
	private void OnScoreTimerTimeout()
	{
		_score++;
	}
	private void OnStartTimerTimeout()
	{
		_mobSpawnTimer.Start();
		_scoreTimer.Start();
	}
	private void OnMenuStartGame()
	{
		NewGame();
	}
	private void OnPlayerDeath()
	{
		Player.SetPhysicsProcess(false);
		_isGameOver = true;
	}
}
