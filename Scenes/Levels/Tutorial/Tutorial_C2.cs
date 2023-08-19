using Godot;

public partial class Tutorial_C2 : Level
{
	public override void _Ready()
	{
		// Trigger tutorial message for this room.
	}

	public override void Initialize()
	{
		SetPlayerStart(new(1, 6));

		SetTerrainTiles(new() {
            // Top wall: (0, 2) to (6, 2).
            new(0, 2), new(1, 2), new(2, 2), new(3, 2), new(4, 2), new(5, 2), new(6, 2),

            // Left wall: (0, 3) to (0, 4) and (0, 7) to (0, 8).
            new(0, 3), new(0, 4), new(0, 7), new(0, 8),

            // Bottom wall: (0, 9) to (6, 9).
            new(0, 9), new(1, 9), new(2, 9), new(3, 9), new(4, 9), new(5, 9), new(6, 9),

            // Right wall: (6, 3) to (6, 8).
            new(6, 3), new(6, 4), new(6, 5), new(6, 6), new(6, 7), new(6, 8)
		});

		SetEnemyTiles(new(){
			new(5, 4), new(13, 7), new(15, 2)
		});

		SetDoorTiles(new() {
			// Left to B2.
			{ new(0, 5), "res://Scenes/Levels/Tutorial/Tutorial_B2.tscn" },
			{ new(0, 6), "res://Scenes/Levels/Tutorial/Tutorial_B2.tscn" },
		});
	}
}
