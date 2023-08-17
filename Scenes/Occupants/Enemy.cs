using Godot;

[GlobalClass]
public partial class Enemy : Unit, IOccupant
{
    private readonly Grid _grid = ResourceLoader.Load("res://Resources/Grid.tres") as Grid;
    private Vector2I _cell = Vector2I.Zero;
    private Texture2D _texture;
    private bool _isInCombat = false;

    public Enemy(Vector2I cell, Texture2D texture) : base(cell, texture)
    {
        _cell = cell;
        _texture = texture;
    }

    public override int GetRange() { return 4; }

    public void SetIsInCombat(bool value)
    {
        _isInCombat = value;
    }
}
