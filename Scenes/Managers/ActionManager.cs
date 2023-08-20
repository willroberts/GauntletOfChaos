using Godot;
using Godot.Collections;

public partial class ActionManager : Node2D
{
	public Dictionary<string, Array<Vector2I>> GetActions(System.Collections.Generic.Dictionary<Vector2I, IOccupant> neighbors)
	{
		Dictionary<string, Array<Vector2I>> actions = new();
		actions["attack"] = new();
		actions["chest"] = new();
		actions["switch"] = new();

		foreach ((Vector2I cell, IOccupant neighbor) in neighbors)
		{
			if (neighbor is Enemy)
			{
				actions["attack"].Add(cell);
				GD.Print("You can attack an enemy at ", cell);
			}
			else if (neighbor is Chest)
			{
				actions["chest"].Add(cell);
				GD.Print("You can open a chest at ", cell);
			}
			else if (neighbor is Switch)
			{
				actions["switch"].Add(cell);
				GD.Print("You can operate a switch at ", cell);
			}
		}

		return actions;
	}
}
