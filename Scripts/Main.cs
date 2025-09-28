using Equipment;
using Godot;
using Mobs;
/// <summary>
/// The main class that handles the game logic, including spawning mobs and managing the game state.
/// </summary>
/// <remarks>
/// This will need to be broken down into a level class eventually and proper game management.
/// </remarks>
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
	[Export] private Timer _pickupSpawnTimer;
	[Export] private Timer _scoreTimer;
	[Export] private Timer _startTimer;
	[ExportGroup("Spawnables")]
	[ExportSubgroup("Mobs")]
	[Export] public PackedScene[] MobScenes { get; private set; }
	[Export] private PathFollow2D _mobSpawner;
	[ExportSubgroup("Pickups")]
	[Export] public PackedScene[] PickupScenes { get; private set; }
	[Export] private Path2D _pickupPath;
	[Export] private PathFollow2D _pickupSpawner;
	private int _score = 0;
	private double _pickupTimerDefaultWaitTime;
	private Vector2 _distantBetweenPickupAndPlayer;
	private bool _isGameOver = false;
	public override void _Ready()
	{
		_distantBetweenPickupAndPlayer = Player.Position - _pickupPath.Position;
		_pickupTimerDefaultWaitTime = _pickupSpawnTimer.WaitTime;
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
			_pickupPath.Position = Player.Position - _distantBetweenPickupAndPlayer;
		}
	}
	public void GameOver()
	{
		_mobSpawnTimer.Stop();
		_scoreTimer.Stop();
		_pickupSpawnTimer.Stop();
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
		GetTree().CallGroup("Pickups", Node.MethodName.QueueFree);
		_score = 0;
		_startTimer.Start();
		Ui.NewGame(_startTimer.WaitTime);
	}
	private void OnMobTimerTimeout()
	{
		Mob mob = (Mob)MobScenes[GD.Randi() % MobScenes.Length].Instantiate();
		_mobSpawner.ProgressRatio = GD.Randf();
		Vector2 directionToPlayer = (Player.Position - _mobSpawner.Position).Normalized();
		float baseSpeed = (float)GD.RandRange(150.0, 250.0);
    	float mobSpeedModifier = mob.Speed + (int)(GD.RandRange(-1.0, 0.5) % mob.Speed);
		mob.Position = _mobSpawner.GlobalPosition;
		mob.LinearVelocity = directionToPlayer * (baseSpeed + mobSpeedModifier);
		AddChild(mob);
	}
	private void OnPickupTimerTimeout()
	{
		GD.Print($"Pickup path position: {_pickupPath.Position}");
		GD.Print($"Pickup spawner name: {_pickupSpawner.Name}");
		GD.Print($"Pickup spawner position: {_pickupSpawner.Position}");

		Pickup pickup = (Pickup)PickupScenes[GD.Randi() % PickupScenes.Length].Instantiate();
		_pickupSpawner.ProgressRatio = GD.Randf();
		pickup.Position = _pickupSpawner.GlobalPosition;
		AddChild(pickup);

		GD.Print($"Pickup spawned at: {pickup.Position}");
	}
	private void OnScoreTimerTimeout()
	{
		_score++;
		if (_score % 10 == 0)
			_pickupSpawnTimer.WaitTime += 0.25f;
	}
	private void OnStartTimerTimeout()
	{
		_mobSpawnTimer.Start();
		_pickupSpawnTimer.WaitTime = _pickupTimerDefaultWaitTime;
		_pickupSpawnTimer.Start();
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
