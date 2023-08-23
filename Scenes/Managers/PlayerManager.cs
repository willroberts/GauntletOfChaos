using Godot;
using Godot.Collections;

public partial class PlayerManager : Node2D
{
	private Player _player;
	private InventoryManager _inventory = new();

	public void Create(Vector2I cell, Texture2D texture)
	{
		if (_player != null)
		{
			GD.Print("Error: Player already exists!");
			return;
		}

		_player = new(cell, texture);
		AddChild(_player);
	}

	public Player Get()
	{
		return _player;
	}

	public static Dictionary<string, Array<Vector2I>> GetActions(System.Collections.Generic.Dictionary<Vector2I, IOccupant> neighbors)
	{
		Dictionary<string, Array<Vector2I>> actions = new()
		{
			["attack"] = new(),
			["chest"] = new(),
			["switch"] = new()
		};

		foreach ((Vector2I cell, IOccupant neighbor) in neighbors)
		{
			if (neighbor is Enemy)
			{
				actions["attack"].Add(cell);
				GD.Print("Debug[PlayerManager]: You can attack an enemy at ", cell);
			}
			else if (neighbor is Chest)
			{
				actions["chest"].Add(cell);
				GD.Print("Debug[PlayerManager]: You can open a chest at ", cell);
			}
			else if (neighbor is Switch)
			{
				actions["switch"].Add(cell);
				GD.Print("Debug[PlayerManager]: You can activate a switch at ", cell);
			}
		}

		return actions;
	}
}
