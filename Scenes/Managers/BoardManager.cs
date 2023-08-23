using Godot;
using System.Collections.Generic;
using System.Text.Json;
using NativeDict = System.Collections.Generic.Dictionary<Godot.Vector2I, IOccupant>;
using SerializableDict = System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<int>>;

public partial class BoardManager : Node2D
{
	[Signal]
	public delegate void MoveFinishedEventHandler(Vector2I newCell);

	private Grid _grid = ResourceLoader.Load("res://Resources/Grid.tres") as Grid;
	private BoardLayer _unitLayer = new();
	private TileMap _highlightTiles;
	private TileMap _pathTiles;
	private Vector2I _hoveredCell;

	public override void _Ready()
	{
		_unitLayer.MoveFinished += OnMoveFinished;
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		// Handle mouse click / touch.
		if (@event is InputEventMouseButton btn && btn.ButtonIndex == MouseButton.Left && btn.Pressed)
		{
			Vector2I target = _grid.ScreenToGrid(btn.Position);
			GD.Print("Debug[BoardManager]: Clicked on ", target);
			ProcessClick(target);
			GetViewport().SetInputAsHandled();
		}
		else if (@event is InputEventMouseMotion evt)
		{
			Vector2I cell = _grid.Clamp(_grid.ScreenToGrid(evt.Position));
			if (cell.Equals(_hoveredCell)) { return; }
			_hoveredCell = cell;
			ProcessHover(_hoveredCell);
			GetViewport().SetInputAsHandled();
		}
	}

	/*
	* Serialization
	*/

	// Serialize the current Board as a JSON string.
	public string Serialize()
	{
		// Define arrays to store cell coordinates for various occupant types.
		List<int> enemyCells = new();
		List<int> chestCells = new();
		List<int> openedChestCells = new();
		List<int> switchCells = new();
		List<int> activatedSwitchCells = new();
		List<int> gateCells = new();

		// Iterate the board, appending coordinates to the above arrays.
		foreach ((Vector2I key, IOccupant value) in _unitLayer.GetAllOccupants())
		{
			// TODO: Differentiate enemies? Or let them spawn randomly on load?
			if (value is Enemy) { enemyCells.Add(_grid.ToIndex(key)); }
			else if (value is Chest c)
			{
				if (!c.IsOpened()) { chestCells.Add(_grid.ToIndex(key)); }
				else { openedChestCells.Add(_grid.ToIndex(key)); }
			}
			else if (value is Switch s)
			{
				if (!s.IsActivated()) { switchCells.Add(_grid.ToIndex(key)); }
				else { activatedSwitchCells.Add(_grid.ToIndex(key)); }
			}
			else if (value is Gate g)
			{
				if (!g.IsOpened()) { gateCells.Add(_grid.ToIndex(key)); }
			}
		}

		// Assemble the arrays into a string-keyed dictionary.
		SerializableDict assembled = new(){
			{ "enemies", enemyCells },
			{ "chests", chestCells },
			{ "openedChests", openedChestCells },
			{ "switches", switchCells },
			{ "activatedSwitches", activatedSwitchCells },
			{ "gates", gateCells }
		};

		// Serialize and return the dictionary.
		return JsonSerializer.Serialize(assembled);
	}

	public NativeDict Deserialize(string serialized)
	{
		SerializableDict input = JsonSerializer.Deserialize<SerializableDict>(serialized);
		NativeDict result = new();

		// Convert the serializable format back to what we use in the Grid engine.
		foreach ((string key, List<int> values) in input)
		{
			foreach (int v in values)
			{
				Vector2I cell = _grid.FromIndex(v);
				switch (key)
				{
					case "enemies":
						result[cell] = new Enemy(cell, null); // FIXME: Texture.
						break;
					case "chests":
						result[cell] = new Chest(cell, null); // FIXME: Texture.
						break;
					case "openedChests":
						Chest c = new(cell, null); // FIXME: Texture.
						c.SetIsOpened(true);
						result[cell] = c;
						break;
					case "switches":
						result[cell] = new Switch(cell, null); // FIXME: Texture.
						break;
					case "activatedSwitches":
						Switch s = new(cell, null); // FIXME: Texture.
						s.SetIsActivated(true);
						result[cell] = s;
						break;
					case "gates":
						result[cell] = new Gate(cell, null); // FIXME: Texture;
						break;
					default:
						GD.Print("Error: Unknown deserialization key ", key);
						break;
				}
			}
		}

		// TODO: Apply this to the board instead of returning a value.
		return result;
	}

	/*
	* Board management
	*/

	public void ConfigureTiles(TileMap highlightTiles, TileMap pathTiles)
	{
		if (highlightTiles != null)
		{
			_highlightTiles = highlightTiles;
			_highlightTiles.ZIndex = (int)ZOrder.Highlight;
			_unitLayer.HighlightTiles = _highlightTiles;
		}

		if (pathTiles != null)
		{
			_pathTiles = pathTiles;
			_pathTiles.ZIndex = (int)ZOrder.Path;
			_unitLayer.PathTiles = _pathTiles;
		}
	}

	public void ClearBoard()
	{
		_unitLayer.Clear();

		foreach (Node n in GetChildren())
		{
			if (n is Enemy || n is Gate || n is Chest || n is Switch)
			{
				RemoveChild(n);
			}
		}
	}

	public void InitializeBoard(Level level, TextureManager textureCache)
	{
		ClearBoard();
		SpawnStaticContent(level, textureCache);
		SpawnDynamicContent(level, textureCache);
	}

	private void SpawnStaticContent(Level level, TextureManager textureCache)
	{
		foreach (Vector2I cell in level.GetTerrainTiles())
		{
			Terrain t = new(cell);
			t.ZIndex = (int)ZOrder.Level;
			AddOccupant(t, cell);
		}
		foreach (Vector2I cell in level.GetNPCTiles())
		{
			NPC n = new(cell);
			n.ZIndex = (int)ZOrder.Units;
			AddOccupant(n, cell);
			//AddChild(n); // Disabled: NPCs don't have textures yet.
		}
	}

	private void SpawnDynamicContent(Level level, TextureManager textureCache)
	{
		foreach (Vector2I cell in level.GetEnemyTiles())
		{
			Enemy e = new(cell, textureCache.Get("enemy_rat"));
			e.ZIndex = (int)ZOrder.Units;
			AddOccupant(e, cell);
			AddChild(e);
		}
		bool condition = false;
		foreach (Vector2I cell in level.GetGateTiles())
		{
			// DEBUG: Skip adding gates until switches are in.
			if (condition)
			{
				Gate g = new(cell, textureCache.Get("prop_gate"));
				g.ZIndex = (int)ZOrder.Items;
				AddOccupant(g, cell);
				AddChild(g);
			}
		}
		foreach (Vector2I cell in level.GetSwitchTiles())
		{
			Switch s = new(cell, textureCache.Get("prop_switch"));
			s.ZIndex = (int)ZOrder.Items;
			AddOccupant(s, cell);
			AddChild(s);
		}
		foreach (Vector2I cell in level.GetChestTiles())
		{
			Chest c = new(cell, textureCache.Get("prop_chest"));
			c.ZIndex = (int)ZOrder.Items;
			AddOccupant(c, cell);
			AddChild(c);
		}
	}

	public void DestroyGates()
	{
		foreach (Node n in GetChildren())
		{
			if (n is Gate g)
			{
				RemoveOccupant(g.GetCell());
				RemoveChild(n);
			}
		}
	}

	public void AddOccupant(IOccupant occupant, Vector2I cell)
	{
		_unitLayer.Add(occupant, cell);
	}

	public void RemoveOccupant(Vector2I cell)
	{
		_unitLayer.ClearCell(cell);
	}

	public void SetHighlightTilesEnabled(bool bEnable)
	{
		if (bEnable)
		{
			_unitLayer.HighlightTiles = _highlightTiles;
			return;
		}

		_unitLayer.HighlightTiles = null;
	}

	public Dictionary<Vector2I, IOccupant> GetNeighboringOccupants(Vector2I cell)
	{
		return _unitLayer.GetNeighbors(cell);
	}

	/*
	* Event handlers
	*/

	public void ProcessClick(Vector2I cell)
	{
		_unitLayer.HandleClick(cell);
	}

	public void ProcessHover(Vector2I cell)
	{
		_unitLayer.HandleHover(cell);
	}

	// Propagate this signal to Main with no changes.
	public void OnMoveFinished(Vector2I newCell)
	{
		EmitSignal("MoveFinished", newCell);
	}
}