using Godot;

public partial class Tutorial_A2 : Level
{
	public override void _Ready()
	{
		// Trigger tutorial message for this room.
	}

	public override void Initialize()
	{
		SetPlayerStart(new(18, 6));

		SetTerrainTiles(new() {
            // Top wall: (13, 2) to (19, 2).
            new(13, 2), new(14, 2), new(15, 2), new(16, 2), new(17, 2), new(18, 2), new(19, 2),

            // Left wall: (13, 3) to (13, 8).
            new(13, 3), new(13, 4), new(13, 5), new(13, 6), new(13, 7), new(13, 8),

            // Bottom wall: (13, 9) to (19, 9).
            new(13, 9), new(14, 9), new(15, 9), new(16, 9), new(17, 9), new(18, 9), new(19, 9),

            // Right wall: (19, 3) to (19, 4) and (19, 7) to (19, 8).
            new(19, 3), new(19, 4), new(19, 7), new(19, 8)
		});

		SetDoorTiles(new() {
			// Right to B2.
			{ new(19, 5), "res://Scenes/Levels/Tutorial/Tutorial_B2.tscn" },
			{ new(19, 6), "res://Scenes/Levels/Tutorial/Tutorial_B2.tscn" },
		});
	}
}
