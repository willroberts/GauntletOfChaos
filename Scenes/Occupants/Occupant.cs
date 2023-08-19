using Godot;

// Occupant implements the IOccupant interface in a Node2D container.
// Occupants may be static (e.g. chests) or movable (e.g. units).
public partial class Occupant : Node2D, IOccupant
{
	// Occupants have a corresponding grid cell and a sprite.
	private Grid _grid = ResourceLoader.Load("res://Resources/Grid.tres") as Grid;
	private Vector2I _cell;
	private Sprite2D _sprite;

	// Occupants must be initialized with their desired location.
	public Occupant(Vector2I cell, Texture2D texture)
	{
		_cell = cell;
		if (texture != null) { AddSprite(texture); }
	}

	// When the Occupant has been initialized, set its position.
	public override void _Ready()
	{
		Position = _grid.GridToScreen(GetCell());
	}

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
		_sprite = new Sprite2D()
		{
			Texture = texture,
			ZIndex = (int)ZOrder.Occupants,
			TextureFilter = TextureFilterEnum.Nearest,
			Scale = new(4F, 4F)
		};
		AddChild(_sprite);
	}

	// RemoveSprite clears the current sprite
	public virtual void RemoveSprite()
	{
		if (_sprite != null)
		{
			RemoveChild(_sprite);
			_sprite = null;
		}
	}
}