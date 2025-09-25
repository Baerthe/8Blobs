using Godot;
using System;
/// <summary>
/// The main class that handles the game logic, including spawning mobs and managing the game state.
/// </summary>
public partial class Main : Node2D
{
	[ExportGroup("Singles")]
	[ExportSubgroup("Core")]
	[Export] public Player Player { get; private set; }
	[Export] public Marker2D PlayerStart { get; private set; }
	[Export] public Camera2D Camera { get; private set; }
	[ExportSubgroup("UI")]
	[Export] public Label ScoreLiteral { get; private set; }
	[Export] public Label HealthLiteral { get; private set; }

	[ExportSubgroup("Timers")]
	[Export] public Timer MobSpawnTimer { get; private set; }
	[Export] public Timer ScoreTimer { get; private set; }
	[Export] public Timer StartTimer { get; private set; }
	[ExportGroup("Spawnables")]
	[ExportSubgroup("Mobs")]
	[Export] public PackedScene[] MobScenes { get; private set; }
	private int _score = 0;
	public override void _Ready()
	{
		Player.Start(PlayerStart.Position);
	}
	public override void _Process(double delta)
	{
		Camera.Position = Player.Position;
		ScoreLiteral.Text = _score.ToString("D8");
		HealthLiteral.Text = Player.Health.ToString("D2");
	}
	public void GameOver()
	{

	}
	public void NewGame()
	{
		_score = 0;
		StartTimer.Start();
	}
	private void OnMobTimerTimeout()
	{
		// init
	}
	private void OnScoreTimerTimeout()
	{
		_score++;
	}
	private void OnStartTimerTimeout()
	{
		MobSpawnTimer.Start();
		ScoreTimer.Start();
	}
}
