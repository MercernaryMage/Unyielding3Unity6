using System;
using System.Collections.Generic;
using UnityEngine;

public class DebugForceCharacterSetup : MonoBehaviour
{
	[Serializable]
	public class HeroLoadout
	{
		public List<ItemScriptableObject> weapons = new List<ItemScriptableObject>();
		public List<ItemScriptableObject> advantages = new List<ItemScriptableObject>();
	}

	public List<HeroLoadout> loadouts = new List<HeroLoadout>(4)
	{
		new HeroLoadout(),
		new HeroLoadout(),
		new HeroLoadout(),
		new HeroLoadout()
	};

	void Update()
	{
		if (!Application.isEditor)
		{
			return;
		}
		bool shiftHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
		if (shiftHeld && Input.GetKeyDown(KeyCode.D))
		{
			ApplyLoadouts();
		}
	}

	void ApplyLoadouts()
	{
		List<StorageCharacter> heroes = PersistenceManager.Instance.currentTeam;
		for (int i = 0; i < heroes.Count; ++i)
		{
			if (i >= loadouts.Count)
			{
				break;
			}
			StorageCharacter sc = heroes[i];
			if (sc == null)
			{
				continue;
			}
			HeroLoadout loadout = loadouts[i];

			// Clear existing weapons, advantages, and equipment
			for (int s = 0; s < sc.slots.Length; ++s)
			{
				sc.slots[s].weapon = null;
			}
			sc.advantages.Clear();
			sc.equipment.Clear();

			// Add the loadout weapons into slots
			for (int j = 0; j < loadout.weapons.Count && j < sc.slots.Length; ++j)
			{
				if (loadout.weapons[j] != null)
				{
					Item newItem = Item.CreateItem(loadout.weapons[j]);
					sc.slots[j].weapon = newItem;
					sc.equipment.Add(newItem);
				}
			}

			// Add the loadout advantages
			foreach (ItemScriptableObject advantageDef in loadout.advantages)
			{
				if (advantageDef == null)
				{
					continue;
				}
				sc.advantages.Add(advantageDef);
				sc.equipment.Add(Item.CreateItem(advantageDef));
			}
		}
	}
}
