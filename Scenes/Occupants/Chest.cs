using Godot;

public partial class Chest : Occupant
{
	private bool _isOpened = false;

	public Chest(Vector2I cell, Texture2D texture) : base(cell, texture) { }

	public bool IsOpened() { return _isOpened; }

	public void SetIsOpened(bool value) { _isOpened = value; }

	public void Open()
	{
		if (_isOpened)
		{
			GD.Print("Error: Cannot open already-opened chest!");
			return;
		}

		_isOpened = true;
	}
}