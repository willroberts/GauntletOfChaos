using Godot;

public partial class LevelManager : Node2D
{
    private Level _currentLevel;

    public void Load(Level level)
    {
        level.Initialize();
        _currentLevel = level;
    }

    public Level CurrentLevel() { return _currentLevel; }

    // OnMoved is a callback which fires when a unit finishes moving.
    // Used to detect when a gateway is reached, so we can change the level.
    public virtual void OnMoved(Vector2I newCell)
    {
        if (!_currentLevel.GetGatewayTiles().Contains(newCell)) { return; }
        GD.Print("Player entered a gateway!");
    }
}