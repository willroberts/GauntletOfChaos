using Godot;
using Godot.Collections;

public partial class TextureManager : Node2D
{
	private Dictionary<string, Texture2D> _textureCache = new();

	public override void _Ready()
	{
		Initialize();
	}

	public Texture2D Get(string textureName)
	{
		if (!_textureCache.ContainsKey(textureName))
		{
			GD.Print("Error: Texture ", textureName, " is not present in the texture cache.");
			return null;
		}

		return _textureCache[textureName];
	}

	private void Initialize()
	{
		// Player.
		_textureCache.Add("player_knight", LoadTexture("Assets/TinyDungeon/Tiles/tile_0097.png"));
		// Props.
		_textureCache.Add("prop_gate", LoadTexture("Assets/TinyDungeon/Tiles/tile_0077.png"));
		_textureCache.Add("prop_switch", LoadTexture("Assets/TinyDungeon/Tiles/tile_0125.png"));
		_textureCache.Add("prop_chest", LoadTexture("Assets/TinyDungeon/Tiles/tile_0089.png"));
		_textureCache.Add("prop_chest_opened", LoadTexture("Assets/TinyDungeon/Tiles/tile_0091.png"));
		// Enemies.
		_textureCache.Add("enemy_bat", LoadTexture("Assets/TinyDungeon/Tiles/tile_0120.png"));
		_textureCache.Add("enemy_spider", LoadTexture("Assets/TinyDungeon/Tiles/tile_0122.png"));
		_textureCache.Add("enemy_rat", LoadTexture("Assets/TinyDungeon/Tiles/tile_0123.png"));
	}

	private static Texture2D LoadTexture(string filename)
	{
		return ResourceLoader.Load(filename) as Texture2D;
	}
}
