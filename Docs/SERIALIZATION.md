# Game State Serialization

Each room in the dungeon must be serializeable, so we can save and load its
state when entering or exiting the room, respectively.

This enables us to avoid undesired behavior such as respawning enemies when
re-entering a room the player already cleared, or respawning chests the player
already opened.

## Serialization Requirements and Format

The `DungeonManager` will keep track of the state for every room in the
Dungeon using a `Dictionary<cell, occupant>`.

We currently use a `Dictionary<Godot.Vector2I, IOccupant>` to represent a board
layer. Unfortunately, neither Godot types nor Interface types can be
serialized.

The hierarchy for a Dungeon serialized in JSON might look like this:

- Dungeon
  - Rooms (each subkey contains an array of cell indices in int format)
    - Enemies (destroyed? simply don't include them)
	- Chests
	- OpenedChests (still rendered, not interactable)
	- Switches
	- ActivatedSwitches (still rendered, not interactable)
	- Gates (unlocked? simply don't include them)