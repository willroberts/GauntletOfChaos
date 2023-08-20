using Godot;

public partial class Town : Level
{
	public override void Initialize()
	{
		SetPlayerStart(new(9, 4));

		SetTerrainTiles(new() {
			// Waypoint.
			new(8, 0), new(9, 0), new(10, 0), new(11, 0),
			new(8, 1), new(11, 1),
			new(8, 2), new(11, 2),

			// Weapon vendor.
			new(2, 1), new(3, 1), new(4, 1), new(5, 1),
			new(2, 2), new(3, 2), new(4, 2), new(5, 2),
			new(2, 3), new(3, 3), new(4, 3), new(5, 3),
			new(2, 4),

			// Magic vendor.
			new(14, 1), new(15, 1), new(16, 1), new(17, 1),
			new(14, 2), new(15, 2), new(16, 2), new(17, 2),
			new(14, 3), new(15, 3), new(16, 3), new(17, 3),

			// Gambling vendor.
			new(2, 7), new(3, 7), new(4, 7), new(5, 7),
			new(2, 8), new(3, 8), new(4, 8), new(5, 8),
			new(2, 9), new(3, 9), new(4, 9), new(5, 9),

			// Identification vendor.
			new(14, 7), new(15, 7), new(16, 7), new(17, 7),
			new(14, 8), new(15, 8), new(16, 8), new(17, 8),
			new(14, 9), new(15, 9), new(16, 9), new(17, 9)
		});

		SetPortalTiles(new() { new(9, 1), new(10, 1) });

		Level tutorialLevel = GD.Load<PackedScene>("res://Scenes/Levels/Tutorial/Tutorial_B3.tscn").Instantiate() as Level;
		SetPortalConnections(new()
		{
			{ "Tutorial Dungeon", tutorialLevel }
		});

		SetNPCTiles(new() {
			// Weapon vendor.
			new(3, 4),
			// Magic vendor.
			new(16, 4),
			// Gambling vendor.
			new(4, 10),
			// Identification vendor.
			new(15, 10)
		});
	}

	public override bool IsTown() { return true; }
}