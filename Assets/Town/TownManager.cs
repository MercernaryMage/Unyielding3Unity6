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
		SceneManager.LoadScene("Overland");
	}

	public void ForwardClicked()
	{
		ToOverland();
	}

	string GetPlayerWarning()
	{
		//Empty weapons
		//not maxed out weight
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
