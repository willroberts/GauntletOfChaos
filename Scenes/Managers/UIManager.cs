using Godot;
using Godot.Collections;

public partial class UIManager : Node2D
{
	[Signal]
	public delegate void LevelSelectedEventHandler(Level targetLevel);

	// TODO: Implement selectable cursor size (Small:32x32, Medium:48x48, Large:64x64).
	private readonly Resource _cursorDefault = GD.Load("res://Assets/OpenGameArt/Cursor/cursor48.png");
	private readonly Resource _cursorClick = GD.Load("res://Assets/OpenGameArt/Cursor/cursor48_down.png");
	private PortalMenu _portalMenu;
	private Label _turnPopup = new();
	private Timer _turnPopupTimer = new();
	private Theme _largeTextTheme = GD.Load("res://Resources/Themes/Londrina96.tres") as Theme;

	public override void _Ready()
	{
		ZIndex = (int)ZOrder.UI;
		InitializePortalMenu();
		InitializeTurnPopupTimer();
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

	public void InitializeTurnPopupTimer()
	{
		_turnPopupTimer = new();
		_turnPopupTimer.WaitTime = 2;
		_turnPopupTimer.OneShot = true;
		_turnPopupTimer.Timeout += OnTurnPopupTimerFinished;
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

	public void ShowTurnPopup(string text)
	{
		GD.Print("Debug[UIManager:ShowTurnPopup] Showing turn popup with text: ", text);

		// Draw text.
		//_turnPopup.Theme = _largeTextTheme;
		//_turnPopup.SetAnchorsPreset(Control.LayoutPreset.Center);
		//_turnPopup.Position = new(450, 300); // Use x=395 for "Enemies' turn!" text.
		//_turnPopup.ZIndex = (int)ZOrder.UI;
		//_turnPopup.Text = text;
		//AddChild(_turnPopup);

		//_turnPopupTimer.Start();
	}

	private void OnTurnPopupTimerFinished()
	{
		RemoveChild(_turnPopup);
	}

	public void OnLevelSelected(Level targetLevel)
	{
		// Signal passthrough from LevelSelected to Main.
		EmitSignal("LevelSelected", targetLevel);
	}

	public void OnTurnStart(int whoseTurn)
	{
		GD.Print("Debug[UIManager] Received TurnStart signal.");
		switch (whoseTurn)
		{
			case 0:
				ShowTurnPopup("Your turn!");
				break;
			case 1:
				ShowTurnPopup("Enemies' turn!");
				break;
			default:
				GD.Print("Debug[UIManager] Error: Invalid whoseTurn value ", whoseTurn);
				break;
		}
	}
}