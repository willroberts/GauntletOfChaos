using Godot;

public partial class LevelManager : Node2D
{
	[Signal]
	public delegate void LevelChangedEventHandler();

	private Level _currentLevel;

	public Level GetCurrentLevel() { return _currentLevel; }

	// TODO: Differentiate between first load and further loads.
	public void ChangeLevel(Level newLevel)
	{
		// Unload any existing level.
		if (_currentLevel != null) { RemoveChild(_currentLevel); }
		_currentLevel = null;

		// Load the new level.
		_currentLevel = newLevel;
		_currentLevel.ZIndex = (int)ZOrder.Level;
		_currentLevel.Initialize();
		AddChild(_currentLevel);

		// Signal other Managers.
		EmitSignal("LevelChanged");
	}
}
