using Godot;

public partial class Chest : Node2D, IOccupant
{
    private Vector2I _cell;
    private Texture2D _texture; // 89
    private Texture2D _openedTexture; // 91

    public Chest(Vector2I cell) { _cell = cell; }
    public Vector2I GetCell() { return _cell; }
    public int GetRange() { return 0; }
    public bool ReadyToMove() { return false; }

    public void Open()
    {

    }
}