using Godot;
using Godot.Collections;

enum ZOrder { Level, Highlight, Items, Path, Occupants, Units, UI };

public partial class Main : Node2D
{
	/*
	* Configurable attributes.
	*/

	[Export]
	public Grid Grid = ResourceLoader.Load("res://Resources/Grid.tres") as Grid;

	[Export]
	public TileMap HighlightTiles;

	[Export]
	public TileMap PathTiles;

	[Export]
	public PackedScene InitialLevel;

	/*
	* Managers and Components.
	*/

	BoardManager _boardManager;
	TextureManager _textureManager;

	/*
	* Private attributes
	*/

	//private readonly BoardLayer _unitLayer = new();
	private Level _currentLevel;
	private Vector2I _hoveredCell = Vector2I.Zero;
	private Player _player;
	private readonly string _dungeonSelectScene = "res://Scenes/UI/DungeonSelect.tscn";
	private DungeonSelect _dungeonSelectMenu;

	/*
	* Actions (TODO: Move to ActionManager class)
	*/

	private bool _action_canAttack = false;
	private Array<Vector2I> _action_attackCandidates = new();
	private bool _action_canOpenChest = false;
	private Array<Vector2I> _action_chestCandidates = new();
	private bool _action_canOperateSwitch = false;
	private Array<Vector2I> _action_switchCandidates = new();

	/*
	* Core events.
	*/

	public override void _Ready()
	{
		// Initialize managers with no dependencies.
		_textureManager = GetNode<TextureManager>("TextureManager");
		_boardManager = GetNode<BoardManager>("BoardManager");
		_boardManager.InitializeBoard(HighlightTiles, PathTiles);
		_boardManager.SetHighlightTilesEnabled(false);

		//
		//ConfigureHighlightTiles();
		//ConfigurePathTiles();
		//EndCombat();
		ChangeLevel(InitialLevel.Instantiate() as Level);
		AddChild(_player);
		ConfigureHUD();
	}

	public override void _Input(InputEvent @event)
	{
		// Handle mouse click / touch.
		if (@event is InputEventMouseButton btn && btn.ButtonIndex == MouseButton.Left && btn.Pressed)
		{
			Vector2I target = Grid.ScreenToGrid(btn.Position);
			GD.Print("Debug: Clicked on ", target);
			//_unitLayer.HandleClick(target);
			_boardManager.ProcessClick(target);
			return;
		}

		// Handle mouse motion.
		if (@event is InputEventMouseMotion evt)
		{
			Vector2I hoveredCell = Grid.Clamp(Grid.ScreenToGrid(evt.Position));
			if (hoveredCell.Equals(_hoveredCell)) { return; }
			_hoveredCell = hoveredCell;
			//_unitLayer.HandleHover(_hoveredCell);
			_boardManager.ProcessHover(_hoveredCell);
			return;
		}
	}

	/*
	* Signal handlers.
	*/

	private void OnPlayerMoved(Vector2I cell)
	{
		// Show the dungeon select menu when a portal tile is entered.
		if (_currentLevel.GetPortalTiles().Contains(cell))
		{
			GD.Print("Debug: Player stepped into a portal");
			_dungeonSelectMenu.Visible = true;
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
		DetermineValidActions();
	}

	private void OnDungeonSelected(Level targetLevel)
	{
		_dungeonSelectMenu.Visible = false;
		ChangeLevel(targetLevel);
	}

	/*
	* Scene engine.
	*/

	private void ChangeLevel(Level targetLevel)
	{
		if (_currentLevel != null) { RemoveChild(_currentLevel); }

		_currentLevel = targetLevel;
		AddChild(_currentLevel);

		_currentLevel.ZIndex = (int)ZOrder.Level;
		//_unitLayer.Clear();
		_boardManager.ClearBoard();
		_currentLevel.Initialize();
		InitializeBoard();
	}

	/*
	* Configuration helpers.
	*/

	private void ConfigureHighlightTiles()
	{
		if (HighlightTiles != null)
		{
			HighlightTiles.ZIndex = (int)ZOrder.Highlight;
		}

		// Start with highlight tiles in a disabled state.
		// They will be enabled when combat begins.
		EndCombat();
	}

	private void ConfigurePathTiles()
	{
		if (PathTiles != null)
		{
			//PathTiles.ZIndex = (int)ZOrder.Path;
			//_unitLayer.PathTiles = PathTiles;
		}
	}

	// TODO: Move to BoardManager class.
	private void InitializeBoard()
	{
		//_unitLayer.Clear();
		_boardManager.ClearBoard();

		// Mark non-navigable tiles as terrain.
		foreach (Vector2I cell in _currentLevel.GetTerrainTiles())
		{
			//_unitLayer.Add(new Terrain(cell), cell);
			_boardManager.AddOccupant(new Terrain(cell), cell);
		}

		// If the current level has NPCs, load them.
		foreach (Vector2I cell in _currentLevel.GetNPCTiles())
		{
			//_unitLayer.Add(new NPC(cell), cell);
			_boardManager.AddOccupant(new NPC(cell), cell);
		}

		// Add the player to the board.
		if (_player == null) { CreatePlayer(); }
		_player.OnMoved(_currentLevel.GetPlayerStart());
		//_unitLayer.Add(_player, _currentLevel.GetPlayerStart());
		_boardManager.AddOccupant(_player, _currentLevel.GetPlayerStart());

		// Add enemies to the board.
		foreach (Node n in GetChildren()) { if (n is Enemy) { RemoveChild(n); } }
		if (_currentLevel.GetEnemyTiles().Count > 0) { StartCombat(); }
		foreach (Vector2I cell in _currentLevel.GetEnemyTiles())
		{
			Enemy e = new(cell, _textureManager.Get("enemy_rat")) { ZIndex = (int)ZOrder.Units };
			//_unitLayer.Add(e, cell);
			_boardManager.AddOccupant(e, cell);
			// TODO: Move children to BoardManager as well, since the board is there.
			AddChild(e);
		}

		// Add chests to the board.

		// Add switches to the board.

		// Add gates to the board.
		foreach (Node n in GetChildren()) { if (n is Gate) { RemoveChild(n); } }
		foreach (Vector2I cell in _currentLevel.GetGateTiles())
		{
			Gate g = new(cell, _textureManager.Get("prop_gate")) { ZIndex = (int)ZOrder.Items };
			//_unitLayer.Add(g, cell);
			_boardManager.AddOccupant(g, cell);
			AddChild(g);
		}

		// Debugging: Destroy gates for now, until enemies and switches are implemented.
		foreach (Vector2I cell in _currentLevel.GetGateTiles())
		{
			foreach (Node n in GetChildren())
			{
				if (n is Gate g)
				{
					//_unitLayer.ClearCell(g.GetCell());
					_boardManager.RemoveOccupant(g.GetCell());
					RemoveChild(n);
				}
			}
		}
	}

	private void CreatePlayer()
	{
		_player = new(_currentLevel.GetPlayerStart(), _textureManager.Get("player_knight"));
		//_unitLayer.MoveFinished += _player.OnMoved;
		_boardManager.OccupantMoved += _player.OnMoved;
	}

	private void ConfigureHUD()
	{
		// Initialize the menu system.
		PackedScene scn = GD.Load<PackedScene>(_dungeonSelectScene);
		_dungeonSelectMenu = scn.Instantiate() as DungeonSelect;
		_dungeonSelectMenu.ZIndex = (int)ZOrder.UI;

		// Prepare connected levels.
		// FIXME: Set these connections by emitting signals from the Level classes.
		Level tutorialLevel = GD.Load<PackedScene>("res://Scenes/Levels/Tutorial/Tutorial_B3.tscn").Instantiate() as Level;
		tutorialLevel.ZIndex = (int)ZOrder.Level;
		_dungeonSelectMenu.SetButtonValue(0, "Tutorial Dungeon", tutorialLevel);
		_dungeonSelectMenu.DungeonSelected += OnDungeonSelected;

		// Subscribe to movement-based HUD events.
		//_unitLayer.MoveFinished += OnPlayerMoved;
		_boardManager.OccupantMoved += OnPlayerMoved;

		// Start in a hidden state.
		_dungeonSelectMenu.Visible = false;
		AddChild(_dungeonSelectMenu);
	}

	/*
	* Additional helpers
	*/

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

	/*
	* Combat component
	*/

	private void StartCombat()
	{
		if (_player == null || _boardManager == null) { return; }

		GD.Print("Debug: Starting combat.");
		_player.SetIsInCombat(true);
		//_unitLayer.HighlightTiles = HighlightTiles;
		_boardManager.SetHighlightTilesEnabled(true);
	}

	private void EndCombat()
	{
		if (_player == null || _boardManager == null) { return; }

		GD.Print("Debug: Ending combat.");
		_player.SetIsInCombat(false);
		//_unitLayer.HighlightTiles = null;
		_boardManager.SetHighlightTilesEnabled(false);
	}

	/*
	* Actions component
	*/

	private void DetermineValidActions()
	{
		if (_player == null || _boardManager == null)
		{
			GD.Print("Error: Failed to check neighboring cells due to null object.");
			return;
		}

		ResetActions();

		System.Collections.Generic.Dictionary<Vector2I, IOccupant> neighbors;
		//neighbors = _unitLayer.GetNeighbors(_player.GetCell());
		neighbors = _boardManager.GetNeighboringOccupants(_player.GetCell());
		foreach ((Vector2I cell, IOccupant neighbor) in neighbors)
		{
			if (neighbor is Enemy)
			{
				_action_canAttack = true;
				_action_attackCandidates.Add(cell);
				GD.Print("You can attack an enemy at ", cell);
			}
			if (neighbor is Chest)
			{
				_action_canOpenChest = true;
				_action_chestCandidates.Add(cell);
				GD.Print("You can open a chest at ", cell);
			}
			if (neighbor is Switch)
			{
				_action_canOperateSwitch = true;
				_action_switchCandidates.Add(cell);
				GD.Print("You can operate a switch at ", cell);
			}
		}
	}

	private void ResetActions()
	{
		_action_canAttack = false;
		_action_canOpenChest = false;
		_action_canOperateSwitch = false;
	}
}
