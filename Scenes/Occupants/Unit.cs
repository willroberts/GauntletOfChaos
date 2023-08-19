using Godot;

public partial class Unit : Occupant
{
    private readonly Grid _grid = ResourceLoader.Load("res://Resources/Grid.tres") as Grid;
    private Vector2I _cell = Vector2I.Zero;

    // Constructing a Unit sets its basic property values.
    public Unit(Vector2I cell, Texture2D texture) : base(cell)
    {
        _cell = cell;
        if (texture != null) { AddSprite(texture); }
    }

    // When the Unit has been initialized, set its position and sprite.
    public override void _Ready()
    {
        Position = _grid.GridToScreen(GetCell());
    }

    // When the Unit finishes moving, update its location and position.
    public void OnMoved(Vector2I newCell)
    {
        _cell = newCell;
        Position = _grid.GridToScreen(newCell);
    }
}