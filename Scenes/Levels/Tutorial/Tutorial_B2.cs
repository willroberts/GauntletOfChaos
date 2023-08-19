using Godot;

public partial class Tutorial_B2 : Level
{
	public override void _Ready()
	{
		// Trigger tutorial message for this room.
	}

	public override void Initialize()
	{
		SetPlayerStart(new(9, 10));

		SetTerrainTiles(new() {
			// Top wall, first half.
			new(0, 0), new(1, 0), new(2, 0), new(3, 0), new(4, 0), new(5, 0), new(6, 0), new(7, 0), new(8, 0),

			// Gate enclosure.
			new(8, 1), new(8, 2), new(11, 1), new(11, 2),

			// Top wall, second half.
			new(11, 0), new(12, 0), new(13, 0), new(14, 0), new(15, 0), new(16, 0), new(17, 0), new(18, 0), new(19, 0),

			// Left wall, first half.
			new(0, 1), new(0, 2), new(0, 3), new(0, 4),

			// Left wall, second half.
			new(0, 7), new(0, 8), new(0, 9), new(0, 10),

			// Right wall, first half.
			new(19, 1), new(19, 2), new(19, 3), new(19, 4),

			// Right wall, second half.
			new(19, 7), new(19, 8), new(19, 9), new(19, 10),
			
			// Bottom wall, first half.
			new(0, 11), new(1, 11), new(2, 11), new(3, 11), new(4, 11), new(5, 11), new(6, 11), new(7, 11), new(8, 11),

			// Bottom wall, second half.
			new(11, 11), new(12, 11), new(13, 11), new(14, 11), new(15, 11), new(16, 11), new(17, 11), new(18, 11), new(19, 11)
		});
	}
}
