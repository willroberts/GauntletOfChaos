using Godot;

public partial class Unit : Occupant
{
	private readonly Grid _grid = ResourceLoader.Load("res://Resources/Grid.tres") as Grid;

	public Unit(Vector2I cell, Texture2D texture) : base(cell, texture) { }

	public void Move(Vector2I newCell)
	{
		SetCell(newCell);
		GD.Print("Debug[Unit:Move]: Unit cell is now ", newCell);
		Position = _grid.GridToScreen(newCell);
		GD.Print("Debug[Unit:Move]: Result of GridToScreen(", newCell, ") was ", Position);
	}

	// When the Unit finishes moving, update its location and position.
	public void OnMoveFinished(Vector2I newCell)
	{
		Move(newCell);
	}
}