using Godot;
using System.Collections.Generic;

// TileManager keeps track of non-Level tilemaps, such as those for highlight
// tiles or path tiles.
public partial class BoardManager : Node2D
{
	[Signal]
	public delegate void OccupantMovedEventHandler(Vector2I newCell);

	private BoardLayer _unitLayer = new();
	private TileMap _highlightTiles;
	private TileMap _pathTiles;

	public override void _Ready()
	{
		// Subscribe to BoardLayer's MoveFinished signal.
		_unitLayer.MoveFinished += ProcessOccupantMoved;
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
	}

	public void AddOccupant(IOccupant occupant, Vector2I cell)
	{
		_unitLayer.Add(occupant, cell);
		//AddChild(occupant as Occupant);
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

	public void ProcessOccupantMoved(Vector2I newCell)
	{
		EmitSignal("OccupantMoved", newCell);
	}
}