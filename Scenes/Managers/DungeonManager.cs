using Godot;
using Godot.Collections;

public partial class RoomState : RefCounted
{
	private string _serialized;

	public void SerializeBoardLayer(BoardLayer layer)
	{
		// FIXME: Actually serialize.
		_serialized = layer.ToString();
	}
}

// DungeonManager is responsible for tracking the state of rooms in a Dungeon.
public partial class DungeonManager : RefCounted
{
	private Dictionary<string, RoomState> _stateMap;

	public RoomState LoadRoomState(string room)
	{
		if (!_stateMap.ContainsKey(room))
		{
			GD.Print("Error: Room ", room, " does not exist in the state map!");
			return null;
		}
		return _stateMap[room];
	}

	public void SaveRoomState(string room, RoomState state)
	{
		_stateMap[room] = state;
	}
}
