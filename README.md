# Trial of Chaos

Trial of Chaos is a mobile-first Tactical RPG / Action RPG hybrid developed with Godot.

## Code Structure

- Main
	- BoardManager
	- TextureManager
	- Player
		- ActionManager
	- DungeonManager (switch states, treasure states, enemy states)
		- LevelManager (tilemaps)
	- UIManager (dungeon select, messages, cursors, etc.)

## To Do

- Fix player position being hardcoded per level. It should changed based on how/where you entered.
- Implement gates (enemy and switch triggers)
	- Implement enemies and combat
	- Implement switches (v1: entering tile activates switch; one-time only)
- Implement chests
