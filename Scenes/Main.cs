using System.Diagnostics.CodeAnalysis;
using Godot;

enum ZOrder { Level, Highlight, Items, Path, Units, UI };

public partial class Main : Node2D
{
	/*
	* Public attributes.
	*/

	[Export]
	public PackedScene InitialLevel;

	[Export]
	public TileMap HighlightTiles;

	[Export]
	public TileMap PathTiles;

	/*
	* Private attributes
	*/

	private Grid _grid = ResourceLoader.Load("res://Resources/Grid.tres") as Grid;
	private readonly Board _gameboard = new();
	private readonly BoardLayer _unitLayer = new();
	private LevelManager _levelManager = new();
	private Vector2I _hoveredCell = Vector2I.Zero;
	private DungeonSelect _dungeonSelectMenu;

	// FIXME: Move this.
	private readonly string _playerTexture = "Assets/TinyDungeon/Tiles/tile_0097.png";

	/*
	* Core events.
	*/

	public override async void _Ready()
	{
		GD.Print(0);
		//await ToSignal(_unitLayer, "ready");

		GD.Print(1);
		ConfigureLevelManagerAndLoad();
		GD.Print(5);
		// Prepare connected levels.
		// FIXME: Use signals instead of preloading hardcoded variable names.
		Level tutorialLevel = InitialLevel.Instantiate() as Level;
		tutorialLevel.ZIndex = (int)ZOrder.Level;

		// Configure and hide the dungeon select menu.
		PackedScene scn = GD.Load<PackedScene>("res://Scenes/UI/DungeonSelect.tscn");
		_dungeonSelectMenu = scn.Instantiate() as DungeonSelect;
		_dungeonSelectMenu.ZIndex = (int)ZOrder.UI;
		_dungeonSelectMenu.SetButtonValue(0, "Tutorial Dungeon", tutorialLevel);
		AddChild(_dungeonSelectMenu);
		_dungeonSelectMenu.Visible = false;

		// Configure movement range highlighting.
		if (_levelManager.CurrentLevel().IsTown()) { HighlightTiles = null; }
		if (HighlightTiles != null)
		{
			HighlightTiles.ZIndex = (int)ZOrder.Highlight;
			_unitLayer.HighlightTiles = HighlightTiles;
		}

		// Configure path rendering.
		if (PathTiles != null)
		{
			PathTiles.ZIndex = (int)ZOrder.Path;
			_unitLayer.PathTiles = PathTiles;
		}

		// Populate the grid with occupants.
		_gameboard.AddLayer("units", _unitLayer);
		RegisterTerrain();
		RegisterNPCs();
		SpawnPlayer();
	}

	public override void _Input(InputEvent @event)
	{
		// Handle mouse click / touch.
		if (@event is InputEventMouseButton btn && btn.ButtonIndex == MouseButton.Left && btn.Pressed)
		{
			Vector2I target = _grid.ScreenToGrid(btn.Position);
			//GD.Print("Clicked on ", target);
			_unitLayer.HandleClick(target);
			//GetViewport().SetInputAsHandled();
			return;
		}

		// Handle mouse motion.
		if (@event is InputEventMouseMotion evt)
		{
			Vector2I hoveredCell = _grid.Clamp(_grid.ScreenToGrid(evt.Position));
			if (hoveredCell.Equals(_hoveredCell)) { return; }
			_hoveredCell = hoveredCell;
			_unitLayer.HandleHover(_hoveredCell);
			//GetViewport().SetInputAsHandled();
			return;
		}
	}

	/*
	* Signal handlers.
	*/

	private void OnGatewayEntered()
	{
		_dungeonSelectMenu.Visible = true;
	}

	private void OnDungeonSelected()
	{
		_dungeonSelectMenu.Visible = false;
	}

	/*
	* Configuration helpers.
	*/

	// LevelManager is responsible for initializing and changing levels.
	// It emits the GatewayEntered signal when the player steps on a Gateway tile.
	// CS1061 is reported when Roslyn can't find signal names.
	[SuppressMessage("Compiler", "CS1061")]
	private async void ConfigureLevelManagerAndLoad()
	{
		GD.Print(2);

		GD.Print(3);

		// Handle the LevelManager's GatewayEntered signal.
		_levelManager.GatewayEntered += OnGatewayEntered;

		// LevelManager intercepts the OnMoved signal to check whether or not
		// the player has entered a Gateway tile.
		_unitLayer.MoveFinished += _levelManager.OnMoved;

		// Load the initial scene.
		Town townLevel = InitialLevel.Instantiate() as Town;
		townLevel.ZIndex = (int)ZOrder.Level;
		AddChild(townLevel);
		_levelManager.Load(townLevel);
		GD.Print(4);
	}

	private void SpawnPlayer()
	{
		Texture2D tex = ResourceLoader.Load(_playerTexture) as Texture2D;
		Player player = new(_levelManager.CurrentLevel().GetPlayerStart(), tex);
		_unitLayer.MoveFinished += player.OnMoved;
		_unitLayer.Add(player, player.GetCell());
		AddChild(player);
	}

	private void RegisterNPCs()
	{
		foreach (Vector2I cell in _levelManager.CurrentLevel().GetNPCTiles())
		{
			_unitLayer.Add(new NPC(cell), cell);
		}
	}

	// Mark terrain tiles as not navigable.
	private void RegisterTerrain()
	{
		foreach (Vector2I cell in _levelManager.CurrentLevel().GetTerrainTiles())
		{
			_unitLayer.Add(new Terrain(cell), cell);
		}
	}
}
