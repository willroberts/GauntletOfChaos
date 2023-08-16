using Godot;
using Godot.Collections;

public partial class Town : TileMap
{
	[Export]
	public Vector2I PlayerStart = new(9, 4);

	public Array<Vector2I> GetNPCLocations()
	{
		return new(){
			new(3, 4),
			new(16, 4),
			new(4, 10),
			new(15, 10)
		};
	}

	public void GetTerrainBlockers()
	{

	}
}
