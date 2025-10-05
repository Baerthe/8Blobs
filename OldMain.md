```CS
	// Event handlers
	private void onPulse()
	{
		GD.Print("Pulse received in Main");
		Ui.Update(_delta, Player.Health, _score);
		_pickupPath.Position = Player.Position - ClockManager.OffsetBetweenPickupAndPlayer;
		_mobPath.Position = Player.Position - ClockManager.OffsetBetweenMobAndPlayer;
		GetTree().CallGroup("Mobs", "Update", Player);
		TilingManager.PlayerCrossedBorder(Player);
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
	private void OnMenuStartGame()
	{
		NewGame();
	}
	private void OnPlayerDeath()
	{
		Player.SetPhysicsProcess(false);
		_isGameOver = true;
	}
	private void ClearScreen()
	{
		GetTree().CallGroup("Mobs", "Death");
		GetTree().CallGroup("Pickups", Node.MethodName.QueueFree);
	}
	// Game state management
	public void GameOver()
	{
		ClearScreen();
		if (Input.IsActionPressed("ui_accept"))
			OnMenuStartGame();
	}
	public void NewGame()
	{
		// Send over default timer sets to game manager
		List<float> timerWaitTimes = new List<float>
        {
            MobSpawnTime,
            PickupSpawnTime,
            ScoreTime,
            StartingTime,
        };
		ClockManager.SetTimers(timerWaitTimes);
		// Setup temporary variables
		var distantBetweenPickupAndPlayer = Player.Position - _pickupPath.Position;
		var distantBetweenMobAndPlayer = Player.Position - _mobPath.Position;
		// Initialize game state
		ClockManager.InitGame();
		TilingManager.LoadTiles();
		// Set flags
		_isGameStarted = true;
		_isGameOver = false;
		// Reset player and menu
		Menu.Hide();
		Player.Start(PlayerStart.Position);
		Camera.Position = Player.Position;
		ClearScreen();
		// Start level
		_startTimer.Start();
		Ui.NewGame(_startTimer.WaitTime);
	}