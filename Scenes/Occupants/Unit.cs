using Godot;
using Godot.Collections;

public partial class Unit : Occupant
{
	[Signal]
	public delegate void AnimationFinishedEventHandler();

	private readonly Grid _grid = ResourceLoader.Load("res://Resources/Grid.tres") as Grid;
	private PathFollow2D _pathFollower = new();
	private Curve2D _curve = new();
	private float _moveSpeedPx = 500.0F;

	public Unit(Vector2I cell, Texture2D texture) : base(cell, texture) { }

	public override void _Process(double delta)
	{
		TickMovement(delta);
	}

	public void AnimateMovement(Array<Vector2I> path)
	{
		if (path.Count == 0) { return; }

		_curve.AddPoint(Vector2I.Zero);
		foreach (Vector2I point in path)
		{
			_curve.AddPoint(_grid.GridToScreen(point) - Position);
		}

		SetCell(path[^1]);

		// Enable ticking to animate movement.
		SetProcess(true);
	}

	public void Move(Vector2I newCell)
	{
		SetCell(newCell);
		GD.Print("Debug[Unit:Move]: Unit cell is now ", newCell);
		Position = _grid.GridToScreen(newCell);
	}

	private void TickMovement(double delta)
	{
		// Use the tick function to animate unit movement.
		_pathFollower.Progress += _moveSpeedPx * (float)delta;
		if (_pathFollower.ProgressRatio >= 1.0)
		{
			// Disable ticking until animation starts again.
			SetProcess(false);

			// Reset components.
			_pathFollower.Progress = 0.0F;
			Position = _grid.GridToScreen(GetCell());
			_curve.ClearPoints();

			// Signal the completion of the animation.
			EmitSignal("AnimationFinished");
		}
	}
}