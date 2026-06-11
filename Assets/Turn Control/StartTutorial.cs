using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartTutorial : MonoBehaviour
{
	public List<CharacterOverrideSetup> setups;

	void Start()
	{
		FlowControl.currentLevel = 0;
		FlowControl.mapSetName = "Tutorial";
		SceneManager.LoadScene("Combat");

		//turn off debug start
		DebugStartup.init = true;

		
		PersistenceManager.Instance.SetFlag("WeaponSlotsLocked", true);
		PersistenceManager.Instance.SetFlag("AdvantagesLocked", true);
		PersistenceManager.Instance.SetFlag("EnergyLocked", true);
		

		//override the characters
		CreateCharacters(setups, new List<ItemScriptableObject>());
	}

	public static void CreateCharacters(List<CharacterOverrideSetup> overrideSetups,
										List<ItemScriptableObject> unlockedItems)
	{
		PersistenceManager.Instance.currentTeam.Add(new StorageCharacter(CharacterRepository.Instance.GetCharacter("Lenette")));
		PersistenceManager.Instance.currentTeam.Add(new StorageCharacter(CharacterRepository.Instance.GetCharacter("David")));
		PersistenceManager.Instance.currentTeam.Add(new StorageCharacter(CharacterRepository.Instance.GetCharacter("Dane")));
		PersistenceManager.Instance.currentTeam.Add(new StorageCharacter(CharacterRepository.Instance.GetCharacter("Cotton")));

		for (int i = 0; i < PersistenceManager.Instance.currentTeam.Count; ++i)
		{
			if (i >= overrideSetups.Count) break;

			StorageCharacter sc = PersistenceManager.Instance.currentTeam[i];
			CharacterOverrideSetup setup = overrideSetups[i];

			// Clear existing weapons, advantages, and equipment
			for (int s = 0; s < sc.slots.Length; ++s)
			{
				sc.slots[s].weapon = null;
			}
			sc.advantages.Clear();
			sc.equipment.Clear();

			// Add override weapons into slots
			for (int j = 0; j < setup.weaponSlots.Count; ++j)
			{
				if (setup.weaponSlots[j] != null)
				{
					Item newItem = Item.CreateItem(setup.weaponSlots[j]);
					sc.slots[j].weapon = newItem;
					sc.equipment.Add(newItem);
				}
			}

			// Add override advantages
			foreach (ItemScriptableObject advantageDef in setup.adventages)
			{
				sc.advantages.Add(advantageDef);
				Item item = Item.CreateItem(advantageDef);
				sc.equipment.Add(item);
			}
		}

		foreach (ItemScriptableObject item in unlockedItems)
		{
			PersistenceManager.Instance.unlockedItems.Add(item);
		}
	}
}
