using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemRepository : Singleton<ItemRepository>
{
	ItemCollection itemsReal;
	Dictionary<string, ItemScriptableObject> items;

	private void Awake()
	{
		itemsReal = Resources.Load<GameObject>("ItemCollection").GetComponent<ItemCollection>();
		items = new Dictionary<string, ItemScriptableObject>();
		foreach (ItemScriptableObject item in itemsReal.items)
		{
			items[item.name] = item;
		}
	}

	public ItemScriptableObject GetExactItem(string itemName)
	{
		return items[itemName];
	}

	public  IReadOnlyList<ItemScriptableObject> GetItems()
	{
		return itemsReal.items.AsReadOnly();
	}

	public Sprite GetSprite(Sprite s)
	{
		if (s == null)
		{
			return itemsReal.missingIcon;
		}
		else
		{
			return s;
		}
	}
}
