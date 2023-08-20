# Trial of Chaos

Trial of Chaos is a mobile-first Tactical RPG / Action RPG hybrid developed with Godot.

## Code Structure

- Main
	- BoardManager
	- TextureManager
	- ActionManager
	- DungeonManager (switch states, treasure states, enemy states)
		- LevelManager (tilemaps)

## To Do

- Fix player position being hardcoded per level. It should changed based on how/where you entered.
- Implement gates in code (enemy and switch triggers)
	- Implement enemies and combat
	- Implement switches in code (v1: entering tile activates switch; one-time only)
- Implement chests in code
