using Godot;
using Godot.Collections;

// A Gateway represents a tile in a Level which connects to another Level.
// It has an ID, and a list of connections.
public partial class Gateway : Node
{
	private int _id = -1;
	private Vector2I _cell;
	private Array<Level> _connections = new();

	public Array<Level> GetConnections() { return _connections; }
	public void AddConnection(Level level) { _connections.Add(level); }
	public void RemoveConnection(Level level) { _connections.Remove(level); }
}
