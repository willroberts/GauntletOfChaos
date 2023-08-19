using Godot;

public partial class Tutorial_B1 : Level
{
	public override void _Ready()
	{
		// Trigger tutorial message for this room.
	}

	public override void Initialize()
	{
		SetPlayerStart(new(9, 10));

		SetPortalTiles(new() { new(9, 0), new(10, 0) });

		SetTerrainTiles(new() {
			// Top wall: (0, 0) to (19, 0).
			new(0, 0), new(1, 0), new(2, 0), new(3, 0), new(4, 0), new(5, 0), new(6, 0), new(7, 0), new(8, 0), new(9, 0),
			new(10, 0), new(11, 0), new(12, 0), new(13, 0), new(14, 0), new(15, 0), new(16, 0), new(17, 0), new(18, 0), new(19, 0),

			// Left wall: (0, 1) to (0, 10).
			new(0, 1), new(0, 2), new(0, 3), new(0, 4), new(0, 5), new(0, 6), new(0, 7), new(0, 8), new(0, 9), new(0, 10),

			// Right wall: (19, 1) to (19, 10).
			new(19, 1), new(19, 2), new(19, 3), new(19, 4), new(19, 5), new(19, 6), new(19, 7), new(19, 8), new(19, 9), new(19, 10),

			// Left room: (1, 4) to (4, 4) and (6, 1) to (6, 4).
			new(1, 4), new(2, 4), new(3, 4), new(4, 4), new(6, 1), new(6, 2), new(6, 3), new(6, 4),

			// Right room: (13, 1) to (13, 4) and (15, 4) to (18, 4).
			new(13, 1), new(13, 2), new(13, 3), new(13, 4), new(15, 4), new(16, 4), new(17, 4), new(18, 4),

			// Bottom wall, first part: (0, 11) to (8, 11).
			new(0, 11), new(1, 11), new(2, 11), new(3, 11), new(4, 11), new(5, 11), new(6, 11), new(7, 11), new(8, 11),

			// Bottom wall, second part: (11, 11) to (19, 11).
			new(11, 11), new(12, 11), new(13, 11), new(14, 11), new(15, 11), new(16, 11), new(17, 11), new(18, 11), new(19, 11),

			// Tables.
			new(4, 8), new(9, 2), new(15, 8)
		});

		SetDoorTiles(new() {
			// Down to B2.
			{ new(9, 11), "res://Scenes/Levels/Tutorial/Tutorial_B2.tscn" },
			{ new(10, 11), "res://Scenes/Levels/Tutorial/Tutorial_B2.tscn" }
		});

		SetEnemyTiles(new() { new(3, 8), new(4, 7), new(15, 7), new(16, 8) });

		Resource boss = null;
		SetBossTiles(new() { { new(10, 2), boss } });
	}
}
