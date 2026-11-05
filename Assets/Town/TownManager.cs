using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TownManager : SceneSingleton<TownManager>
{
	public bool hasWarnedPlayer = false;
	public TownPlayerEquipmentWarning equipmentWarning;

	public void ToOverland()
	{
		if (!hasWarnedPlayer)
		{
			string warning = GetPlayerWarning();

			if (warning != "")
			{
				ShowPlayerEquipmentWarning(warning);
				hasWarnedPlayer = true;
				return;
			}
		}
		ReadyCharacters();
		SceneManager.LoadScene("Overland");
	}

	void ReadyCharacters()
	{
		foreach (StorageCharacter storageCharacter in PersistenceManager.Instance.currentTeam)
		{
			storageCharacter.currentDetermination = storageCharacter.characterDefintion.maxDetermination;
			storageCharacter.equipment = new List<Item>();
			foreach (WeaponSlot weaponSlot in storageCharacter.slots)
			{
				if (weaponSlot.weapon != null)
				{
					storageCharacter.equipment.Add(weaponSlot.weapon);
				}
			}
			foreach (ItemScriptableObject itemScriptableObject in storageCharacter.advantages)
			{
				Item item = new Item();
				item.Init(itemScriptableObject);
				storageCharacter.equipment.Add(item);
			}
		}
	}

	public void ForwardClicked()
	{
		ToOverland();
	}

	string GetPlayerWarning()
	{
		if (PersistenceManager.Instance.GetFlag("WeaponSlotsLocked"))
		{
			return "";
		}
		string outStr = "";
		foreach (StorageCharacter storageCharacter in PersistenceManager.Instance.currentTeam)
		{
			foreach (WeaponSlot weaponSlot in storageCharacter.slots)
			{
				if (weaponSlot.weapon == null)
				{
					outStr += $"{storageCharacter.characterDefintion.displayName} has an empty weapon slot\n";
					break;
				}
			}
			int curentWeight = 0;
			foreach (ItemScriptableObject item in storageCharacter.advantages)
			{
				curentWeight += item.weight;
			}
			if (curentWeight < storageCharacter.characterDefintion.weight)
			{
				outStr += $"{storageCharacter.characterDefintion.displayName} can bring more Advantages\n";
			}
		}

		return outStr;
	}

	void ShowPlayerEquipmentWarning(string warning)
	{
		equipmentWarning.Set(warning);
	}
}
