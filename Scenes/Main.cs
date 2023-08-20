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

	private BoardManager _boardManager;
	private ActionManager _actionManager;
	private TextureManager _textureManager;
	private UIManager _uiManager;

	// FIXME: Move to LevelManager.
	private Level _currentLevel;

	// FIXME: Move to PlayerManager.
	private Player _player;

	/*
	* Core events.
	*/

	public override void _Ready()
	{
		_textureManager = GetNode<TextureManager>("TextureManager");

		_boardManager = GetNode<BoardManager>("BoardManager");
		_boardManager.InitializeBoard(HighlightTiles, PathTiles);
		_boardManager.SetHighlightTilesEnabled(false);
		_boardManager.OccupantMoved += OnPlayerMoved;

		_actionManager = GetNode<ActionManager>("ActionManager");

		_uiManager = GetNode<UIManager>("UIManager");
		_uiManager.PortalButtonPressed += OnDungeonSelected;

		// Initialize the Level (TODO: Convert to Manager).
		// Also creates the Player (TODO: Use PlayerManager).
		ChangeLevel(InitialLevel.Instantiate() as Level);
	}

	private void OnPlayerMoved(Vector2I cell)
	{
		// Show the dungeon select menu when a portal tile is entered.
		if (_currentLevel.GetPortalTiles().Contains(cell))
		{
			GD.Print("Debug: Player stepped into a portal");
			_uiManager.ShowPortalMenu();
			return;
		}

		// Change the level when a door tile is entered.
		if (_currentLevel.GetDoorTiles().ContainsKey(cell))
		{
			GD.Print("Debug: Player stepped into a door");
			string targetLevel = _currentLevel.GetDoorTiles()[cell];
			ChangeLevel(LevelFromFile(targetLevel));
			return;
		}

		// Determine which actions the player can take.
		_actionManager.GetActions(_boardManager.GetNeighboringOccupants(cell));
	}

	private void OnDungeonSelected(Level targetLevel)
	{
		_uiManager.HidePortalMenu();
		ChangeLevel(targetLevel);
	}

	private void ChangeLevel(Level targetLevel)
	{
		if (_currentLevel != null) { RemoveChild(_currentLevel); }

		_currentLevel = targetLevel;
		AddChild(_currentLevel);

		_currentLevel.ZIndex = (int)ZOrder.Level;
		_currentLevel.Initialize();
		_boardManager.ClearBoard();
		_boardManager.PopulateBoard(_currentLevel, _textureManager);
		_uiManager.SetPortalChoices(_currentLevel.GetPortalConnections());

		// Add the player to the board.
		if (_player == null) { CreatePlayer(); }
		_player.OnMoved(_currentLevel.GetPlayerStart());
		_boardManager.AddOccupant(_player, _currentLevel.GetPlayerStart());

		if (_currentLevel.GetEnemyTiles().Count > 0) { StartCombat(); }
		else { EndCombat(); }
	}

	private void CreatePlayer()
	{
		_player = new(_currentLevel.GetPlayerStart(), _textureManager.Get("player_knight"));
		_boardManager.OccupantMoved += _player.OnMoved;
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
		if (_player == null || _boardManager == null) { return; }

		GD.Print("Debug: Starting combat.");
		_player.SetIsInCombat(true);
		_boardManager.SetHighlightTilesEnabled(true);
	}

	private void EndCombat()
	{
		if (_player == null || _boardManager == null) { return; }

		GD.Print("Debug: Ending combat.");
		_player.SetIsInCombat(false);
		_boardManager.SetHighlightTilesEnabled(false);
	}
}
