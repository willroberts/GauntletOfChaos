using Godot;

public partial class NPC : Node2D, IOccupant
{
    private Vector2I _cell;

    public NPC(Vector2I cell)
    {
        _cell = cell;
    }

    public Vector2I GetCell() { return _cell; }
    public int GetRange() { return 0; }
    public bool ReadyToMove() { return false; }
}