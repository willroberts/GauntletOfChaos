using Godot;
using Godot.Collections;

public partial class UIManager : Node2D
{
	[Signal]
	public delegate void PortalButtonPressedEventHandler(Level targetLevel);

	private DungeonSelect _portalMenu;

	public override void _Ready()
	{
		InitializePortalMenu();
	}

	public void InitializePortalMenu()
	{
		_portalMenu = GD.Load<PackedScene>("res://Scenes/UI/DungeonSelect.tscn").Instantiate() as DungeonSelect;
		_portalMenu.ZIndex = (int)ZOrder.UI;
		_portalMenu.DungeonSelected += OnPortalButtonPressed;
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
			GD.Print("Setting option ", i, " to ", name, " / ", level);
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

	public void OnPortalButtonPressed(Level targetLevel)
	{
		// Signal passthrough from DungeonSelect to Main.
		EmitSignal("PortalButtonPressed", targetLevel);
	}
}