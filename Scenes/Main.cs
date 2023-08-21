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

	private ActionManager _actionManager;
	private BoardManager _boardManager;
	private DungeonManager _dungeonManager;
	private LevelManager _levelManager;
	private TextureManager _textureManager;
	private UIManager _uiManager;
	private TurnManager _turnManager = new();

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
		// Initialize managers with no dependencies first.
		_actionManager = GetNode<ActionManager>("ActionManager");
		_dungeonManager = GetNode<DungeonManager>("DungeonManager");
		_levelManager = GetNode<LevelManager>("LevelManager");
		_levelManager.LevelChanged += OnLevelChanged;
		_textureManager = GetNode<TextureManager>("TextureManager");
		_uiManager = GetNode<UIManager>("UIManager");
		_uiManager.LevelSelected += OnLevelSelected;

		// Subscribe to turn start signals in the UI.
		_turnManager.TurnStart += _uiManager.OnTurnStart;
		_turnManager.TurnStart += OnTurnStart;

		// Depends on TextureManager.
		_boardManager = GetNode<BoardManager>("BoardManager");
		_boardManager.ConfigureTiles(HighlightTiles, PathTiles);
		_boardManager.SetHighlightTilesEnabled(false);
		_boardManager.MoveFinished += OnMoveFinished;
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
		_actionManager.GetActions(_boardManager.GetNeighboringOccupants(cell));
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
		_boardManager.Initialize(level, _textureManager);
		_uiManager.SetPortalChoices(level.GetPortalConnections());
		_player.Move(level.GetPlayerStart());
		_boardManager.AddOccupant(_player, level.GetPlayerStart());

		GD.Print("Debug[Main:OnLevelChanged]: Set player cell to ", _player.GetCell());
		GD.Print("Debug[Main:OnLevelChanged]: Added player to board at ", level.GetPlayerStart());

		if (level.GetEnemyTiles().Count > 0) { StartCombat(); }
		else { EndCombat(); }
	}
}
