using Godot;

public partial class Main : Node2D
{
	[Export]
	public TileMap HighlightTiles;

	[Export]
	public TileMap PathTiles;

	private Grid _grid = ResourceLoader.Load("res://Resources/Grid.tres") as Grid;
	private readonly Board _gameboard = new();
	private readonly BoardLayer _unitLayer = new();
	private TileMap _currentLevel;
	private Vector2I _hoveredCell = Vector2I.Zero;

	// FIXME: Move this.
	private readonly string _playerTexture = "Assets/TinyDungeon/Tiles/tile_0097.png";

	public override void _Ready()
	{
		_currentLevel = GetChild<TileMap>(0);
		if (_currentLevel.Name == "Town") { HighlightTiles = null; }
		_unitLayer.HighlightTiles = HighlightTiles;

		_unitLayer.PathTiles = PathTiles;
		_gameboard.AddLayer("units", _unitLayer);

		// Instantiate the level manager, and subscribe it to the MoveFinished signal.

		// Populate the grid with occupants.
		RegisterTerrain();
		RegisterNPCs();
		SpawnPlayer();
	}

	private void SpawnPlayer()
	{
		Texture2D tex = ResourceLoader.Load(_playerTexture) as Texture2D;
		Player player = new((_currentLevel as Level).GetPlayerStart(), tex);
		_unitLayer.MoveFinished += player.OnMoved;
		_unitLayer.Add(player, player.GetCell());
		AddChild(player);
	}

	private void RegisterNPCs()
	{
		foreach (Vector2I cell in (_currentLevel as Level).GetNPCTiles())
		{
			_unitLayer.Add(new NPC(cell), cell);
		}
	}

	// Mark terrain tiles as not navigable.
	private void RegisterTerrain()
	{
		foreach (Vector2I cell in (_currentLevel as Level).GetTerrainTiles())
		{
			_unitLayer.Add(new Terrain(cell), cell);
		}
	}

	public override void _Input(InputEvent @event)
	{
		// Handle mouse click / touch.
		if (@event is InputEventMouseButton btn && btn.ButtonIndex == MouseButton.Left && btn.Pressed)
		{
			Vector2I target = _grid.ScreenToGrid(btn.Position);
			GD.Print("Clicked on ", target);
			_unitLayer.HandleClick(target);
			GetViewport().SetInputAsHandled();
			return;
		}

		// Handle mouse motion.
		if (@event is InputEventMouseMotion evt)
		{
			Vector2I hoveredCell = _grid.Clamp(_grid.ScreenToGrid(evt.Position));
			if (hoveredCell.Equals(_hoveredCell)) { return; }
			_hoveredCell = hoveredCell;
			_unitLayer.HandleHover(_hoveredCell);
			GetViewport().SetInputAsHandled();
			return;
		}
	}
}
