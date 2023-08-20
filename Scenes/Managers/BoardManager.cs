using Godot;
using System.Collections.Generic;

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
	* Board management
	*/

	public void InitializeBoard(TileMap highlightTiles, TileMap pathTiles)
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

	public void PopulateBoard(Level level, TextureManager textureCache)
	{
		ClearBoard();

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