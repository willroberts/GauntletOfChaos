using Godot;

public partial class Switch : Occupant
{
	private bool _isActivated = false;

	public Switch(Vector2I cell, Texture2D texture) : base(cell, texture) { }

	public bool IsActivated() { return _isActivated; }

	public void SetIsActivated(bool value) { _isActivated = value; }

	public void Activate()
	{
		if (_isActivated)
		{
			GD.Print("Error: Cannot activate already-activated switch!");
			return;
		}

		_isActivated = true;
	}
}