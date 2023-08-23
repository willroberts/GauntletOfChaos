using Godot;
using System;

// RefCounted because it doesn't need to appear on-screen, and we acan still
// emit signals.
public partial class CombatManager : RefCounted
{
	[Signal]
	public delegate void TurnStartEventHandler(int whoseTurn);

	private enum TurnGroups { Player, Enemies };
	private int _whoseTurn;
	private int _moveCount;
	private int _actionCount;

	public CombatManager()
	{
		_whoseTurn = (int)TurnGroups.Player;
		_moveCount = 0;
		_actionCount = 0;
		EmitSignal("TurnStart", _whoseTurn);
	}

	public bool ShouldEndTurn()
	{
		return _moveCount > 0 && _actionCount > 0;
	}

	public void EndTurn()
	{
		// Increment the turnGroup enum to see who's next.
		int whoseTurnNext = _whoseTurn + 1;
		int turnGroupCount = Enum.GetNames(typeof(TurnGroups)).Length;
		if (whoseTurnNext >= turnGroupCount) { whoseTurnNext -= turnGroupCount; }

		// Update whoseTurn and emit the corresponding signal.
		_whoseTurn = whoseTurnNext;
		EmitSignal("TurnStart", _whoseTurn);
	}
}
