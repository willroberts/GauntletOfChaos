using Godot;
using Godot.Collections;
using System;
using System.Linq;

/*
Random rnd = new Random();
int month  = rnd.Next(1, 13);  // creates a number between 1 and 12
int dice   = rnd.Next(1, 7);   // creates a number between 1 and 6
int card   = rnd.Next(52);     // creates a number between 0 and 51
*/

public partial class LootManager : RefCounted
{
	private int _maxItemsSpawned = 3;

	// Temporary test values.
	private Array<string> _itemNames = new()
	{
		"Short Sword", "Long Sword",
		"Buckler", "Small Shield", "Kite Shield", "Tower Shield"
	};

	public int SpawnGold(int lootLevel)
	{
		Random rnd = new();
		int baseAmount = rnd.Next(1, 9);
		return baseAmount * lootLevel;
	}

	public Array<Item> SpawnItems(int lootLevel, int limit)
	{
		Random rnd = new();
		Array<Item> items = new();

		if (limit < 1) { limit = rnd.Next(1, _maxItemsSpawned); }

		foreach (int i in Enumerable.Range(0, limit))
		{
			string itemName = _itemNames[rnd.Next(0, _itemNames.Count)];
			items.Add(new() { Name = itemName });
		}

		return items;
	}
}