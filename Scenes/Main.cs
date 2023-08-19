using Godot;

enum ZOrder { Level, Highlight, Items, Path, Occupants, Units, UI };

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
    private readonly BoardLayer _unitLayer = new();
    private Level _currentLevel;
    private Vector2I _hoveredCell = Vector2I.Zero;
    private Player _player;
    private Texture2D _playerTexture = ResourceLoader.Load("Assets/TinyDungeon/Tiles/tile_0097.png") as Texture2D;
    private Texture2D _ratTexture = ResourceLoader.Load("Assets/TinyDungeon/Tiles/tile_0123.png") as Texture2D;

    private readonly string _dungeonSelectScene = "res://Scenes/UI/DungeonSelect.tscn";
    private DungeonSelect _dungeonSelectMenu;

    /*
	* Core events.
	*/

    public override void _Ready()
    {
        ConfigureHighlightTiles();
        ConfigurePathTiles();
        ChangeLevel(InitialLevel.Instantiate() as Level);
        AddChild(_player);
        ConfigureHUD();
    }

    public override void _Input(InputEvent @event)
    {
        // Handle mouse click / touch.
        if (@event is InputEventMouseButton btn && btn.ButtonIndex == MouseButton.Left && btn.Pressed)
        {
            Vector2I target = _grid.ScreenToGrid(btn.Position);
            //GD.Print("Clicked on ", target);
            _unitLayer.HandleClick(target);
            return;
        }

        // Handle mouse motion.
        if (@event is InputEventMouseMotion evt)
        {
            Vector2I hoveredCell = _grid.Clamp(_grid.ScreenToGrid(evt.Position));
            if (hoveredCell.Equals(_hoveredCell)) { return; }
            _hoveredCell = hoveredCell;
            _unitLayer.HandleHover(_hoveredCell);
            return;
        }
    }

    /*
	* Signal handlers.
	*/

    private void OnPlayerMoved(Vector2I cell)
    {
        // Show the dungeon select menu when a gateway tile is entered.
        if (_currentLevel.GetGatewayTiles().Contains(cell))
        {
            _dungeonSelectMenu.Visible = true;
        }
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
        _unitLayer.Clear();
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
        _unitLayer.HighlightTiles = null;
    }

    private void ConfigurePathTiles()
    {
        if (PathTiles != null)
        {
            PathTiles.ZIndex = (int)ZOrder.Path;
            _unitLayer.PathTiles = PathTiles;
        }
    }

    private void InitializeBoard()
    {
        // Mark non-navigable tiles as terrain.
        foreach (Vector2I cell in _currentLevel.GetTerrainTiles())
        {
            _unitLayer.Add(new Terrain(cell), cell);
        }

        // If the current level has NPCs, load them.
        foreach (Vector2I cell in _currentLevel.GetNPCTiles())
        {
            _unitLayer.Add(new NPC(cell), cell);
        }

        // Add the player to the board.
        if (_player == null) { CreatePlayer(); }
        _player.OnMoved(_currentLevel.GetPlayerStart());
        _unitLayer.Add(_player, _currentLevel.GetPlayerStart());

        // Add enemies to the board.
        foreach (Vector2I cell in _currentLevel.GetEnemyTiles())
        {
            //Enemy e = new(cell, _ratTexture);
            //e.ZIndex = (int)ZOrder.Units;
            //_unitLayer.Add(e, cell);
            //AddChild(e);
        }

        // Add chests to the board.

        // Add switches to the board.

        // Add gates to the board.
    }

    private void CreatePlayer()
    {
        _player = new(Vector2I.Zero, _playerTexture);
        _unitLayer.MoveFinished += _player.OnMoved;
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
        _unitLayer.MoveFinished += OnPlayerMoved;

        // Start in a hidden state.
        _dungeonSelectMenu.Visible = false;
        AddChild(_dungeonSelectMenu);
    }
}
