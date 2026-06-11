using UnityEngine;

public class Item
{
    public ItemScriptableObject itemDefinition;
	public bool loaded = true;
	public bool used = false;
	public int charges = 0;

	public static Item CreateItem(ItemScriptableObject itemScriptableObject)
	{
		Item item = new Item();
		item.Init(itemScriptableObject);
		return item;
	}

	public void Init(ItemScriptableObject scriptableObject)
	{
		itemDefinition = scriptableObject;
		charges = itemDefinition.charges;
	}

	public void Reset()
	{
		used = false;
	}

	public void Load()
	{
		loaded = true;
	}

	public void Unload()
	{
		loaded = false;
	}
}
