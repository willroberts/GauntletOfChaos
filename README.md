# Trial of Chaos

Trial of Chaos is a mobile-first Tactical RPG / Action RPG hybrid developed with Godot.

## To Do

- Fix player position being hardcoded per level. It should changed based on how/where you entered.
- Implement gates (enemy and switch triggers)
	- Implement enemies and combat
		- AI detection method:
			- BoardLayer needs `MoveSelectionWithin(targetCell, withinCell, withinRange)`.
			- What if there are multiple enemies on the path? Need to compute enemy vision set and compare against it. Then be able to `GetUnitsWithin(cell, range)`
	- Implement switches (v1: entering tile activates switch; one-time only)
- Implement chests
- Encapsulate the generic components of Main as a new Engine class.
	- Add this class to the grid-engine plugin/addon.