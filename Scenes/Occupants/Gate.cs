using Godot;

public partial class Gate : Occupant
{
	private bool _isOpened = false;

	public Gate(Vector2I cell, Texture2D texture) : base(cell, texture) { }

	public bool IsOpened() { return _isOpened; }

	public void SetIsOpened(bool value) { _isOpened = value; }

	public void Open()
	{
		if (_isOpened)
		{
			GD.Print("Error: Cannot open already-opened gate!");
			return;
		}

		_isOpened = true;
	}
}