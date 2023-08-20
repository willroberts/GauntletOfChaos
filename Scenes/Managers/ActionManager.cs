using Godot;
using Godot.Collections;

public partial class ActionManager : Node2D
{
	private bool _canAttack = false;
	private bool _canOpenChest = false;
	private bool _canOperateSwitch = false;
	private bool _canRest = false;

	private Array<Vector2I> _attackTargets = new();
	private Array<Vector2I> _chestTargets = new();
	private Array<Vector2I> _switchTargets = new();

	public override void _Ready()
	{

	}

	public Dictionary<string, Array<Vector2I>> GetActions(System.Collections.Generic.Dictionary<Vector2I, IOccupant> neighbors)
	{
		Reset();
		Dictionary<string, Array<Vector2I>> actions = new();
		actions["attack"] = new();
		actions["chest"] = new();
		actions["switch"] = new();

		foreach ((Vector2I cell, IOccupant neighbor) in neighbors)
		{
			if (neighbor is Enemy)
			{
				_canAttack = true;
				_attackTargets.Add(cell);
				actions["attack"].Add(cell);
				GD.Print("You can attack an enemy at ", cell);
			}
			else if (neighbor is Chest)
			{
				_canOpenChest = true;
				_chestTargets.Add(cell);
				actions["chest"].Add(cell);
				GD.Print("You can open a chest at ", cell);
			}
			else if (neighbor is Switch)
			{
				_canOperateSwitch = true;
				_switchTargets.Add(cell);
				actions["switch"].Add(cell);
				GD.Print("You can operate a switch at ", cell);
			}
		}

		return actions;
	}

	public void Reset()
	{
		_canAttack = false;
		_canOpenChest = false;
		_canOperateSwitch = false;
		_canRest = false;

		_attackTargets = new();
		_chestTargets = new();
		_switchTargets = new();
	}
}
