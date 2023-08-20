# Signals

May be out of date. See git history.

## Unit Movement

When the player clicks to confirm a target tile, the board handles the click
and moves the `Occupant` in its internal state accordingly. However, the
occupant itself also must keep track of its own location.

This is achieved with signals:

1. `BoardLayer` emits `MoveFinished`.
1. `BoardManager` propagates `MoveFinished` to `Main`.

`Main:OnMoveFinished()` subscribes to `MoveFinished` and:
- Checks for doors and portals in the new tile.
- Prints possible actions to the console.

`Unit:Move()` subscribes to `MoveFinished` in `Main` and:
- Updates the player's coords with `SetCell()` (not board coords).
- Updates the player's screen position.

## Level Change

1. `UIManager` emits `LevelSelected`.

`Main:OnLevelSelected` subscribes to `LevelSelected` and:
- Closes the portal menu.
- Changes the level to the selected level.

1. `LevelManager` emits `LevelChanged`.

`Main:OnLevelChanged` subscribes to `LevelChanged` and:
- Populates the board with occupants from the level.
- Configures the portal menu.
- Creates the Player if null (PlayerManager this out)
- Updates the Player's location (improve this API with PM).
- 