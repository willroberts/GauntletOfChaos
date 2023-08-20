using Godot;
using Godot.Collections;

/*
  public override void _Input(InputEvent @event)
  {
    if (@event is InputEventMouseButton btn && btn.ButtonIndex == MouseButton.Left)
    {
      if (btn.Pressed) { Input.SetCustomMouseCursor(_cursorClick); }
      else { Input.SetCustomMouseCursor(_cursorDefault); }
    }
  }
*/

public partial class UIManager : Node2D
{
	[Signal]
	public delegate void LevelSelectedEventHandler(Level targetLevel);

	// TODO: Implement selectable cursor size (Small:32x32, Medium:48x48, Large:64x64).
	private readonly Resource _cursorDefault = GD.Load("res://Assets/OpenGameArt/Cursor/cursor48.png");
	private readonly Resource _cursorClick = GD.Load("res://Assets/OpenGameArt/Cursor/cursor48_down.png");
	private PortalMenu _portalMenu;

	public override void _Ready()
	{
		InitializePortalMenu();
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton btn && btn.ButtonIndex == MouseButton.Left)
		{
			if (btn.Pressed) { Input.SetCustomMouseCursor(_cursorClick); }
			else { Input.SetCustomMouseCursor(_cursorDefault); }
		}
	}

	public void InitializePortalMenu()
	{
		_portalMenu = GD.Load<PackedScene>("res://Scenes/UI/PortalMenu.tscn").Instantiate() as PortalMenu;
		_portalMenu.ZIndex = (int)ZOrder.UI;
		_portalMenu.LevelSelected += OnLevelSelected;
		HidePortalMenu();
		AddChild(_portalMenu);
	}

	public void SetPortalChoices(Dictionary<string, Level> choices)
	{
		if (choices.Count > 3)
		{
			GD.Print("Error: Cannot have more than 3 choices in portal menu.");
			return;
		}

		int i = 0;
		foreach ((string name, Level level) in choices)
		{
			_portalMenu.SetButtonValue(i, name, level);
			i++;
		}
	}

	public void ShowPortalMenu()
	{
		_portalMenu.Visible = true;
		//AddChild(_portalMenu);
	}

	public void HidePortalMenu()
	{
		_portalMenu.Visible = false;
		//RemoveChild(_portalMenu);
	}

	public void OnLevelSelected(Level targetLevel)
	{
		// Signal passthrough from LevelSelected to Main.
		EmitSignal("LevelSelected", targetLevel);
	}
}