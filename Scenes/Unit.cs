using Godot;

public partial class Unit : Node2D, IOccupant
{
    private readonly Grid _grid = ResourceLoader.Load("res://Resources/Grid.tres") as Grid;
    private Vector2I _cell = Vector2I.Zero;
    private readonly Texture2D _texture;

    // Constructing a Unit sets its basic property values.
    public Unit(Vector2I cell, Texture2D texture)
    {
        _cell = cell;
        _texture = texture;
    }

    // When the Unit has been initialized, set its position and sprite.
    public override void _Ready()
    {
        Position = _grid.GridToScreen(GetCell());

        if (_texture != null)
        {
            AddChild(new Sprite2D()
            {
                Texture = _texture,
                ZIndex = 1,
                Scale = new(4F, 4F)
            });
        }
    }

    // Defining these methods implements the IOccupant interface.
    public virtual Vector2I GetCell() { return _cell; }
    public virtual int GetRange() { return 3; }
    public virtual bool ReadyToMove() { return true; }

    // When the Unit finishes moving, update its location and position.
    public void OnMoved(Vector2I newCell)
    {
        _cell = newCell;
        Position = _grid.GridToScreen(newCell);
    }
}