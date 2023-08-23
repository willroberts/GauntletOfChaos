using Godot;

public partial class InventoryManager : RefCounted
{
	private int _gold = 0;

	public InventoryManager()
	{
		// Load inventory from save file if present.
	}

	public void AddGold(int amount) { _gold += amount; }
	public void RemoveGold(int amount)
	{
		if (!HasEnoughGold(amount))
		{
			GD.Print("Error: Gold ", _gold, " is less than amount to remove ", amount);
			return;
		}
		_gold -= amount;
	}
	public bool HasEnoughGold(int amount) { return _gold >= amount; }
}