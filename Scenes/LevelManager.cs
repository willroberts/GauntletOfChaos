using Godot;

public partial class LevelManager : Node2D
{
    // TODO: Replace with `Level` type.
    private TileMap _currentLevel;

    public void SetLevel(TileMap level)
    {
        _currentLevel = level;
    }

    public void InitializeLevel()
    {

    }

    public void ShowLevelSelect()
    {

    }

    public void HideLevelSelect()
    {

    }
}