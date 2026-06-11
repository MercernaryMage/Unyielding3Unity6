using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistenceManager : Singleton<PersistenceManager>
{
    public List<StorageCharacter> currentTeam = new List<StorageCharacter>();
	public Dictionary<string, bool> looseFlags = new Dictionary<string, bool>();
	public List<ItemScriptableObject> unlockedItems = new List<ItemScriptableObject>();

	public StorageCharacter GetStorageCharacter(string characterName)
	{
		foreach (StorageCharacter character in currentTeam)
		{
			if (character.characterDefintion.name == characterName)
			{
				return character;
			}
		}
		return null;
	}

	public void SetFlag(string flagName, bool value)
	{
		looseFlags[flagName] = value;
	}

	public bool GetFlag(string flagName)
	{
		if (!looseFlags.ContainsKey(flagName))
		{
			return false;
		}
		return looseFlags[flagName];
	}

	public void AddUnlockedItem(string itemName)
	{
		ItemScriptableObject itemScriptableObject = ItemRepository.Instance.GetExactItem(itemName);
		unlockedItems.Add(itemScriptableObject);
	}
}
