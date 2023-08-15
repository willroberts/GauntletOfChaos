using Godot;

public partial class Player : Unit, IOccupant
{
    private readonly Grid _grid = ResourceLoader.Load("res://Resources/Grid.tres") as Grid;
    private Vector2I _cell = Vector2I.Zero;
    private readonly Texture2D _texture;
    private bool _isInCombat = false;

    public Player(Vector2I cell, Texture2D texture) : base(cell, texture)
    {
        _cell = cell;
        _texture = texture;
    }

    public override void _Ready()
    {
        base._Ready();
        GD.Print("Spawned a player.");
    }

    public override int GetRange()
    {
        if (_isInCombat) { return 3; }
        return _grid.Size.X * _grid.Size.Y; // Practically unlimited.
    }
}