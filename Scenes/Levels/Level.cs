using Godot;
using Godot.Collections;

public partial class Level : TileMap
{
	// Initialize handles initial setup for the level and its contents.
	// Override to change behavior.
	public virtual void Initialize() { }

	// PlayerStart is the cell in which the player should be spawned.
	// It is intentionally not initialized to force the user to set it.
	private Vector2I _PlayerStart;
	public void SetPlayerStart(Vector2I cell) { _PlayerStart = cell; }
	public Vector2I GetPlayerStart()
	{
		return _PlayerStart;
	}

	// TerrainTiles contains the coordinates of tiles which can't be walked on.
	private Array<Vector2I> _TerrainTiles = new();
	public void SetTerrainTiles(Array<Vector2I> tiles) { _TerrainTiles = tiles; }
	public Array<Vector2I> GetTerrainTiles() { return _TerrainTiles; }

	// DoorTiles contains the coordinates of tiles containing simple doors.
	// Since each tile corresponds to a connected Level, this is a Dictionary.
	private Dictionary<Vector2I, string> _DoorTiles = new();
	public void SetDoorTiles(Dictionary<Vector2I, string> tiles) { _DoorTiles = tiles; }
	public Dictionary<Vector2I, string> GetDoorTiles() { return _DoorTiles; }

	// PortalTiles contains the coordinates of portals to other levels, e.g. portals.
	private Array<Vector2I> _PortalTiles = new();
	public void SetPortalTiles(Array<Vector2I> tiles) { _PortalTiles = tiles; }
	public Array<Vector2I> GetPortalTiles() { return _PortalTiles; }

	// PortalConnections contains the names and scenes of connected Levels.
	private Dictionary<string, Level> _PortalConnections = new();
	public void SetPortalConnections(Dictionary<string, Level> conns) { _PortalConnections = conns; }
	public Dictionary<string, Level> GetPortalConnections() { return _PortalConnections; }

	// NPCTiles contains the coordinates of any NPCs.
	private Array<Vector2I> _NPCTiles = new();
	public void SetNPCTiles(Array<Vector2I> tiles) { _NPCTiles = tiles; }
	public Array<Vector2I> GetNPCTiles() { return _NPCTiles; }

	// EnemyTiles contains the coordinates of any enemies.
	private Array<Vector2I> _EnemyTiles = new();
	public void SetEnemyTiles(Array<Vector2I> tiles) { _EnemyTiles = tiles; }
	public Array<Vector2I> GetEnemyTiles() { return _EnemyTiles; }

	// BossTiles contains the coordinates and types of boss enemies.
	private Dictionary<Vector2I, Resource> _BossTiles = new();
	public void SetBossTiles(Dictionary<Vector2I, Resource> tiles) { _BossTiles = tiles; }
	public Dictionary<Vector2I, Resource> GetBossTiles() { return _BossTiles; }

	// GateTiles contains the coordinates of conditional gates.
	private Array<Vector2I> _GateTiles = new();
	public void SetGateTiles(Array<Vector2I> tiles) { _GateTiles = tiles; }
	public Array<Vector2I> GetGateTiles() { return _GateTiles; }

	// SwitchTiles contains the coordinates of interactable switches.
	private Array<Vector2I> _SwitchTiles = new();
	public void SetSwitchTiles(Array<Vector2I> tiles) { _SwitchTiles = tiles; }
	public Array<Vector2I> GetSwitchTiles() { return _SwitchTiles; }

	// ChestTiles contains the coordinates of treasure chests.
	private Array<Vector2I> _ChestTiles = new();
	public void SetChestTiles(Array<Vector2I> tiles) { _ChestTiles = tiles; }
	public Array<Vector2I> GetChestTiles() { return _ChestTiles; }

	// IsTown tracks whether an area should use combat rules or town rules.
	// Override to change behavior.
	public virtual bool IsTown() { return false; }
}
