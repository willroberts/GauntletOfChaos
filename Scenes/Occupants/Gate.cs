using Godot;

public partial class Gate : Node2D, IOccupant
{
    private Vector2I _cell;

    public Gate(Vector2I cell)
    {
        _cell = cell;
    }

    public Vector2I GetCell() { return _cell; }
    public int GetRange() { return 0; }
    public bool ReadyToMove() { return false; }
}