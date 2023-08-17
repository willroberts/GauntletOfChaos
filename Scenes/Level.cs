using Godot;
using Godot.Collections;

public partial class Level : TileMap
{
    // Initialize handles initial setup for the level and its contents.
    // Override to change behavior.
    public virtual void Initialize() { }

    // PlayerStart is the cell in which the player should be spawned.
    private Vector2I _PlayerStart;
    public void SetPlayerStart(Vector2I cell) { _PlayerStart = cell; }
    public Vector2I GetPlayerStart() { return _PlayerStart; }

    // TerrainTiles contains the coordinates of tiles which can't be walked on.
    private Array<Vector2I> _TerrainTiles;
    public void SetTerrainTiles(Array<Vector2I> tiles) { _TerrainTiles = tiles; }
    public Array<Vector2I> GetTerrainTiles() { return _TerrainTiles; }

    // GatewayTiles contains the coordinates of gateways to other levels, e.g. doors.
    private Array<Vector2I> _GatewayTiles;
    public void SetGatewayTiles(Array<Vector2I> tiles) { _GatewayTiles = tiles; }
    public Array<Vector2I> GetGatewayTiles() { return _GatewayTiles; }

    // NPCTiles contains the coordinates of any NPCs.
    private Array<Vector2I> _NPCTiles;
    public void SetNPCTiles(Array<Vector2I> tiles) { _NPCTiles = tiles; }
    public Array<Vector2I> GetNPCTiles() { return _NPCTiles; }

    // IsTown tracks whether an area should use combat rules or town rules.
    // Override to change behavior.
    public virtual bool IsTown() { return false; }
}