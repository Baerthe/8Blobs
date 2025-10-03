using Equipment;
using Godot;
using Mobs;
using Core;
using Core.Interface;
/// <summary>
/// The main class that handles orchestratesion and dependency management of the game.
/// </summary>
/// <remarks>
/// This will need to be broken down into a level class eventually and proper game management.
/// Should move to godot's variation on Singletons using AutoLoad, etc.
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
	[Export] private Path2D _mobPath;
	[Export] private PathFollow2D _mobSpawner;
	[ExportSubgroup("Pickups")]
	[Export] public PackedScene[] PickupScenes { get; private set; }
	[Export] private Path2D _pickupPath;
	[Export] private PathFollow2D _pickupSpawner;
	// Managers Singletons
	private static ITilingManager _tilingManager { get; set; }
	private static IGameManager _gameManager { get; set; }
	// Flags
	private bool _isGameOver = false;
	private bool _isGameStarted = false;
	private double _delta;
	public override void _Ready()
	{
		// Do we have everything?
		NullCheck();
		// Setup variables
		var _distantBetweenPickupAndPlayer = Player.Position - _pickupPath.Position;
		var _distantBetweenMobAndPlayer = Player.Position - _mobPath.Position;
		var _pickupTimerDefaultWaitTime = _pickupSpawnTimer.WaitTime;
		// Setup managers
		_gameManager = new GameManager(Ui, _distantBetweenPickupAndPlayer, _distantBetweenMobAndPlayer);
		_tilingManager = new TilingManager(GetNode<TileMapLayer>("ForegroundLayer"), GetNode<TileMapLayer>("BackgroundLayer"));
		// Register to signals
		_gameManager.PulseTimeout += onPulse;
		// Time for some kids.
		AddChild(_gameManager as Node);
		AddChild(_tilingManager as Node);
		// Main menu is ready to go.
		_gameManager.MenuShow();
	}
	public override void _Process(double delta)
	{
		_delta = delta;
		if (!_isGameStarted) return;
		if (_isGameOver)
		{
			GameOver();
		}
		else
		{
			Camera.Position = Player.Position;
			// Only update some things every few frames to save on performance
			_skipFrameCounter++;
			if (_skipFrameCounter >= _skipPollRate)
			{
				_skipFrameCounter = 0;
				_pickupPath.Position = Player.Position - _distantBetweenPickupAndPlayer;
				_mobPath.Position = Player.Position - _distantBetweenMobAndPlayer;
				GetTree().CallGroup("Mobs", "Update", Player);
				_tilingManager.PlayerCrossedBorder(Player);
			}
		}
		if (delta > 0)
			_timeElapsed += (float)delta;
	}
	public void GameOver()
	{
		ClearScreen();
		_mobSpawnTimer.Stop();
		_scoreTimer.Stop();
		_pickupSpawnTimer.Stop();
		if (Input.IsActionPressed("ui_accept"))
			OnMenuStartGame();
	}
	public void NewGame()
	{
		_isGameStarted = true;
		_isGameOver = false;
		Menu.Hide();
		Player.Start(PlayerStart.Position);
		Camera.Position = Player.Position;
		ClearScreen();
		_timeElapsed = 0.0f;
		_score = 0;
		_startTimer.Start();
		Ui.NewGame(_startTimer.WaitTime);
	}
	private void ClearScreen()
	{
		GetTree().CallGroup("Mobs", "Death");
		GetTree().CallGroup("Pickups", Node.MethodName.QueueFree);
	}
	private void NullCheck()
	{
		if (Player == null) GD.PrintErr("Player not set in Main");
		if (PlayerStart == null) GD.PrintErr("PlayerStart not set in Main");
		if (Camera == null) GD.PrintErr("Camera not set in Main");
		if (Ui == null) GD.PrintErr("Ui not set in Main");
		if (_mobSpawnTimer == null) GD.PrintErr("MobSpawnTimer not set in Main");
		if (_pickupSpawnTimer == null) GD.PrintErr("PickupSpawnTimer not set in Main");
		if (_scoreTimer == null) GD.PrintErr("ScoreTimer not set in Main");
		if (_startTimer == null) GD.PrintErr("StartTimer not set in Main");
		if (MobScenes == null || MobScenes.Length == 0) GD.PrintErr("MobScenes not set in Main");
		if (_mobPath == null) GD.PrintErr("MobPath not set in Main");
		if (_mobSpawner == null) GD.PrintErr("MobSpawner not set in Main");
		if (PickupScenes == null || PickupScenes.Length == 0) GD.PrintErr("PickupScenes not set in Main");
		if (_pickupPath == null) GD.PrintErr("PickupPath not set in Main");
		if (_pickupSpawner == null) GD.PrintErr("PickupSpawner not set in Main");
		if (Menu == null) GD.PrintErr("Menu not set in Main");
	}
	private void onPulse()
	{
		GD.Print("Pulse received in Main");
		Ui.Update(_delta, Player.Health, _score, _isGameOver);
	}
	private void OnMobTimerTimeout()
	{
		Mob mob = (Mob)MobScenes[GD.Randi() % MobScenes.Length].Instantiate();
		_mobSpawner.ProgressRatio = GD.Randf();
		mob.Spawn(Player.Position, _mobSpawner);
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
		_tilingManager.LoadTiles();
	}
	private void OnPlayerDeath()
	{
		Player.SetPhysicsProcess(false);
		_isGameOver = true;
	}
}
