# Gauntlet of Chaos

Gauntlet of Chaos is a mobile-first Tactical RPG / Action RPG hybrid developed with Godot.

## To Do

- Player position is hardcoded per level. Should changed based on where you enter.
		- AI detection method:
			- BoardLayer needs `MoveSelectionWithin(targetCell, withinCell, withinRange)`.
			- What if there are multiple enemies on the path? Need to compute enemy vision set and compare against it. Then be able to `GetUnitsWithin(cell, range)`
- Encapsulate the generic components of Main as a new Engine class.
	- Add this class to the grid-engine plugin/addon.
