using Godot;

[GlobalClass]
public partial class Player : Unit, IOccupant
{
	private readonly Grid _grid = ResourceLoader.Load("res://Resources/Grid.tres") as Grid;
	private bool _isInCombat = false;

	public Player(Vector2I cell, Texture2D texture) : base(cell, texture) { }

	public override int GetRange()
	{
		if (_isInCombat) { return 3; }
		return _grid.Size.X * _grid.Size.Y; // Practically unlimited.
	}

	// TODO: Set this using BeginCombat / EndCombat signals.
	public void SetIsInCombat(bool value)
	{
		_isInCombat = value;
	}

	// Override Occupant methods.
	public override bool ReadyToMove() { return true; }
}
