using Godot;

public partial class Main : Node2D
{
	// FIXME: Move this.
	private readonly string _playerTexture = "Assets/TinyDungeon/Tiles/tile_0097.png";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SpawnPlayer();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void SpawnPlayer()
	{
		Vector2I startPosition;
		if (GetChild<TileMap>(0).Name == "Town") { startPosition = new(9, 4); }
		else { startPosition = new(1, 1); }

		Texture2D tex = ResourceLoader.Load(_playerTexture) as Texture2D;
		AddChild(new Player(startPosition, tex));
	}
}
