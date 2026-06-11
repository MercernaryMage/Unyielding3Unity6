using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponSlot
{
	public Item weapon;
}

public class StorageCharacter
{
	public Character createdCharacter;
	public CharacterScriptableObject characterDefintion;
	public WeaponSlot[] slots = new WeaponSlot[3]
	{
		new WeaponSlot(),
		new WeaponSlot(),
		new WeaponSlot()
	};
	public List<ItemScriptableObject> advantages;
	public List<Item> equipment = new List<Item>();

	//per fight data
	public int currentDetermination;
	public int surgeIndex;
	//per fight data

	public StorageCharacter(CharacterScriptableObject defintion)
	{
		characterDefintion = defintion;
		currentDetermination = defintion.maxDetermination;

		advantages = new List<ItemScriptableObject>(defintion.advantages);

		for (int i = 0; i < slots.Length; ++i)
		{
			if (defintion.weapons[i].item != null)
			{
				slots[i].weapon = Item.CreateItem(defintion.weapons[i].item);
				equipment.Add(slots[i].weapon);
			}
		}
		foreach (ItemScriptableObject itemScriptableObject in advantages)
		{
			Item item = new Item();
			item.Init(itemScriptableObject);
			equipment.Add(item);
		}
	}

	public Item AddItem(ItemScriptableObject itemScriptableObject)
	{
		advantages.Add(itemScriptableObject);
		Item item = new Item();
		item.Init(itemScriptableObject);
		equipment.Add(item);
		return item;
	}
}
