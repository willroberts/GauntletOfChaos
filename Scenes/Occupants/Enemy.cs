using Godot;

[GlobalClass]
public partial class Enemy : Unit, IOccupant
{
	public Enemy(Vector2I cell, Texture2D texture) : base(cell, texture) { }

	// TODO: Create per-Enemy Resources which contain these stats.
	public override int GetRange() { return 4; }

	// Basic AI flow: Ascertain -> Approach -> Ability -> Attack
	public void ProcessTurn()
	{
		Vector2I targetCell = Vector2I.Zero;
		Move(targetCell);
		//Attack();
	}
}
