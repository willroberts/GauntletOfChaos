using Godot;
using System;
using System.Runtime.InteropServices;

// RefCounted because it doesn't need to appear on-screen, and we acan still
// emit signals.
public partial class TurnManager : RefCounted
{
	[Signal]
	public delegate void TurnStartEventHandler(int whoseTurn);

	private enum TurnHavers { Player, Enemies };
	private int _whoseTurn;
	private int _moveCount;
	private int _actionCount;

	public void Initialize()
	{
		_whoseTurn = (int)TurnHavers.Player;
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
		// Increment the turnHaver enum to see who's next.
		int whoseTurnNext = _whoseTurn + 1;
		int turnHaverCount = Enum.GetNames(typeof(TurnHavers)).Length;
		if (whoseTurnNext >= turnHaverCount) { whoseTurnNext -= turnHaverCount; }

		// Update whoseTurn and emit the corresponding signal.
		_whoseTurn = whoseTurnNext;
		EmitSignal("TurnStart", _whoseTurn);
	}
}
