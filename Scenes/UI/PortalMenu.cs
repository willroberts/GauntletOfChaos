using Godot;
using Godot.Collections;

public partial class PortalMenu : Node2D
{
	[Signal]
	public delegate void LevelSelectedEventHandler(Level targetLevel);

	public override void _Ready()
	{
		HideButtons();
	}

	public void HideButtons()
	{
		foreach (Button btn in GetButtons())
		{
			btn.Visible = false;
		}
	}

	public void SetButtonValue(int id, string text, Level targetLevel)
	{
		Array<Button> buttons = GetButtons();
		if (id > buttons.Count - 1)
		{
			GD.Print("Error: Button index ", id, " is out of bounds");
			return;
		}

		// Update the button text, make the button visible, and bind the
		// desired level change to the Pressed delegate.
		Button target = buttons[id];
		target.Text = text;
		target.Visible = true;
		target.Pressed += () => { OnButtonPressed(targetLevel); };
	}

	private Array<Button> GetButtons()
	{
		Array<Button> buttons = new();

		foreach (Node container in GetChildren())
		{
			if (container.Name != "Window") { continue; }
			foreach (Node child in container.GetChildren())
			{
				if (child is Button btn)
				{
					buttons.Add(btn);
				}
			}
		}

		return buttons;
	}

	private void OnButtonPressed(Level targetLevel)
	{
		EmitSignal("LevelSelected", targetLevel);
	}
}
