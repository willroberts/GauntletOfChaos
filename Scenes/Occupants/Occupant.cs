using Godot;

// Occupant implements the IOccupant interface in a Node2D container.
// Occupants may be static (e.g. chests) or movable (e.g. units).
public partial class Occupant : Node2D, IOccupant
{
    // Occupants have a corresponding grid cell.
    private Grid _grid = ResourceLoader.Load("res://Resources/Grid.tres") as Grid;
    private Vector2I _cell;

    // Occupants may also have a sprite and texture.
    private readonly Texture2D _texture;

    // Occupants must be initialized with their desired location.
    public Occupant(Vector2I cell) { _cell = cell; }

    // Occupants implement the most basic form of the IOccupant interface.
    public virtual Vector2I GetCell() { return _cell; }
    public virtual int GetRange() { return 0; }
    public virtual bool ReadyToMove() { return false; }

    // Occupants also have a SetCell() helper which validates input.
    public virtual void SetCell(Vector2I cell)
    {
        if (!_grid.IsWithinBounds(cell))
        {
            GD.Print("Error: Occupant cell is out of bounds.");
            return;
        }

        _cell = cell;
    }

    // AddSprite creates a new sprite with the given texture.
    public virtual void AddSprite(Texture2D texture)
    {
        AddChild(new Sprite2D()
        {
            Texture = texture,
            ZIndex = (int)ZOrder.Occupants,
            TextureFilter = TextureFilterEnum.Nearest,
            Scale = new(4F, 4F)
        });
    }
}