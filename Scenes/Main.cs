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
    private readonly BoardLayer _unitLayer = new();
    private Level _currentLevel;
    private Vector2I _hoveredCell = Vector2I.Zero;
    private Player _player;
    private readonly string _dungeonSelectScene = "res://Scenes/UI/DungeonSelect.tscn";
    private DungeonSelect _dungeonSelectMenu;

    // FIXME: Move this.
    private readonly string _playerTexture = "Assets/TinyDungeon/Tiles/tile_0097.png";

    /*
	* Core events.
	*/

    public override void _Ready()
    {
        ConfigureHighlightTiles();
        ConfigurePathTiles();
        ChangeLevel(InitialLevel.Instantiate() as Level);

        // Spawn the player.
        Texture2D tex = ResourceLoader.Load(_playerTexture) as Texture2D;
        _player = new(_currentLevel.GetPlayerStart(), tex);
        //ResetPlayerPosition();
        _unitLayer.MoveFinished += _player.OnMoved;
        _unitLayer.Add(_player, _player.GetCell());
        AddChild(_player);

        ConfigureHUD();
        GD.Print("Ready!");
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
        _currentLevel.ZIndex = (int)ZOrder.Level;
        _unitLayer.Clear();
        _currentLevel.Initialize();
        InitializeBoard();
        AddChild(_currentLevel);
    }

    private void ResetPlayerPosition()
    {
        _unitLayer.Select(_player.GetCell());
        _unitLayer.MoveSelection(_currentLevel.GetPlayerStart());
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
    }

    private void ConfigureHUD()
    {
        // Initialize the menu system.
        PackedScene scn = GD.Load<PackedScene>(_dungeonSelectScene);
        _dungeonSelectMenu = scn.Instantiate() as DungeonSelect;
        _dungeonSelectMenu.ZIndex = (int)ZOrder.UI;

        // Prepare connected levels.
        // FIXME: Use signals instead of preloading hardcoded variable names.
        Level tutorialLevel = GD.Load<PackedScene>("res://Levels/Tutorial.tscn").Instantiate() as Level;
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
