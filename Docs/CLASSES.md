# Class Hierarchy

- Player
	- Requires TextureManager, BoardManager
	- Required br Main
- BoardManager
	- Requires TextureManager.
	- Required by Main.
- LevelManager
	- No dependencies.
	- Required by Main
- ActionManager
	- No dependencies.
	- Required by Main
- TextureManager
	- No dependencies.
	- Required by Main, BoardManager:PopulateBoard
- UIManager
	- No dependencies.
	- Required by Main