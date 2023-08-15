using Godot;
using System;

public partial class Town : TileMap
{
	private Player _player;
	private Vector2I _playerSpawn = new(9, 4);

	public override void _Ready()
	{
		SpawnPlayer();
	}

	// TODO: Move this to a parent class shared by all levels.
	private void SpawnPlayer()
	{
		Texture2D playerTexture = ResourceLoader.Load("Assets/TinyDungeon/Tiles/tile_0094.png") as Texture2D;
		AddChild(new Player(_playerSpawn, playerTexture));
	}
}
