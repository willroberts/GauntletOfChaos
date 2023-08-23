using Godot;

enum ZOrder { Level, Highlight, Items, Path, Occupants, Units, UI };

public partial class Main : Node2D
{
	[Export]
	public Grid Grid = ResourceLoader.Load("res://Resources/Grid.tres") as Grid;

	[Export]
	public TileMap HighlightTiles;

	[Export]
	public TileMap PathTiles;

	[Export]
	public PackedScene InitialLevel;

	[Export]
	public bool EnableDebugMode = false;

	private BoardManager _boardManager = new();
	private DungeonManager _dungeonManager = new();
	private LevelManager _levelManager = new();
	private PlayerManager _playerManager = new();
	private TextureManager _textureManager = new();
	private TurnManager _turnManager = new();
	private UIManager _uiManager = new();

	// FIXME: Move to PlayerManager.
	private Player _player;

	public override void _Ready()
	{
		InitializeManagers();
		if (_player == null)
		{
			// Depends on BoardManager, TextureManager.
			CreatePlayer();
		}
		_levelManager.ChangeLevel(InitialLevel.Instantiate() as Level);
	}

	private void InitializeManagers()
	{
		// Configure signals.
		_levelManager.LevelChanged += OnLevelChanged;
		_uiManager.LevelSelected += OnLevelSelected;
		_turnManager.TurnStart += _uiManager.OnTurnStart;
		_turnManager.TurnStart += OnTurnStart;
		_boardManager.MoveFinished += OnMoveFinished;

		// Configure the board.
		_boardManager.ConfigureTiles(HighlightTiles, PathTiles);
		_boardManager.SetHighlightTilesEnabled(false);
	}

	private void OnTurnStart(int whoseTurn)
	{
		GD.Print("Debug[Main:OnTurnStart] Received TurnStart signal.");
		GD.Print("Debug[Main:OnTurnStart] Turn ID: ", whoseTurn);
	}

	private void OnMoveFinished(Vector2I cell)
	{
		ProcessPlayerMove(cell);
	}

	private void ProcessPlayerMove(Vector2I cell)
	{
		// Update the Player's cell and screen position.
		_player.Move(cell);

		// Show the dungeon select menu when a portal tile is entered.
		if (_levelManager.GetCurrentLevel().GetPortalTiles().Contains(cell))
		{
			GD.Print("Debug[Main:ProcessPlayerMove]: Player stepped into a portal");
			_uiManager.ShowPortalMenu();
			return;
		}

		// Change the level when a door tile is entered.
		if (_levelManager.GetCurrentLevel().GetDoorTiles().ContainsKey(cell))
		{
			GD.Print("Debug[Main:ProcessPlayerMove]: Player stepped into a door");
			_levelManager.ChangeLevel(LevelFromFile(
				_levelManager.GetCurrentLevel().GetDoorTiles()[cell]
			));
			return;
		}

		// Determine which actions the player can take.
		_playerManager.GetActions(_boardManager.GetNeighboringOccupants(cell));
	}

	private void OnLevelSelected(Level targetLevel)
	{
		_uiManager.HidePortalMenu();
		_levelManager.ChangeLevel(targetLevel);
	}

	private void CreatePlayer()
	{
		_player = new(Vector2I.Zero, _textureManager.Get("player_knight"));
		AddChild(_player);
	}

	private static Level LevelFromFile(string filename)
	{
		if (!filename.StartsWith("res://") || !filename.EndsWith(".tscn"))
		{
			GD.Print("Error: Level filenames must start with res:// end in .tscn");
			return null;
		}

		PackedScene level = GD.Load<PackedScene>(filename);
		if (level == null)
		{
			GD.Print("Error: Failed to load level from ", filename);
			return null;
		}
		return level.Instantiate() as Level;
	}

	private void StartCombat()
	{
		if (_player == null || _boardManager == null)
		{
			GD.Print("Error: Cannot start combat due to nullptr.");
			return;
		}

		_turnManager.Initialize();
		_player.SetIsInCombat(true);
		_boardManager.SetHighlightTilesEnabled(true);
	}

	private void EndCombat()
	{
		if (_player == null || _boardManager == null)
		{
			GD.Print("Error: Cannot end combat due to nullptr.");
			return;
		}

		_player.SetIsInCombat(false);
		_boardManager.SetHighlightTilesEnabled(false);
	}

	private void OnLevelChanged()
	{
		GD.Print("Debug[Main:OnLevelChanged]: --------------");
		GD.Print("Debug[Main:OnLevelChanged]: Level changed.");
		GD.Print("Debug[Main:OnLevelChanged]: --------------");

		Level level = _levelManager.GetCurrentLevel();
		_boardManager.InitializeBoard(level, _textureManager);
		_uiManager.SetPortalChoices(level.GetPortalConnections());
		_player.Move(level.GetPlayerStart());
		_boardManager.AddOccupant(_player, level.GetPlayerStart());

		GD.Print("Debug[Main:OnLevelChanged]: Set player cell to ", _player.GetCell());
		GD.Print("Debug[Main:OnLevelChanged]: Added player to board at ", level.GetPlayerStart());

		if (level.GetEnemyTiles().Count > 0) { StartCombat(); }
		else { EndCombat(); }
	}
}
