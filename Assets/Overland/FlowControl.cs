using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FlowControl : Singleton<FlowControl>
{
	public static int currentLevel = 0;
	public static string mapSetName = "Tutorial";

	public void RunPostFightCode(MapSetScriptableObject mapSet)
	{
		if (!string.IsNullOrEmpty(mapSet.levelCompletedFunction))
		{
			this.GetType().GetMethod(mapSet.levelCompletedFunction).
				Invoke(this, null);
		}
	}

	public void RunSetCompleteCode(MapSetScriptableObject mapSet)
	{
		if (!string.IsNullOrEmpty(mapSet.onComplete))
		{
			this.GetType().GetMethod(mapSet.onComplete).
				Invoke(this, null);
		}
	}

	//----------------------------------------

	public void RunTutorialCode()
	{
		if (currentLevel == 1)
		{
			StorageCharacter davidCharacter = PersistenceManager.Instance.GetStorageCharacter("David");
			davidCharacter.AddItem(ItemRepository.Instance.GetExactItem("Amber Spyglass"));

			StorageCharacter daneCharacter = PersistenceManager.Instance.GetStorageCharacter("Dane");
			daneCharacter.AddItem(ItemRepository.Instance.GetExactItem("Causality Gem"));
		}

		if (currentLevel == 2)
		{
			StorageCharacter lenetteCharacter = PersistenceManager.Instance.GetStorageCharacter("Lenette");
			Item item = new Item();
			item.Init(ItemRepository.Instance.GetExactItem("Shield"));
			lenetteCharacter.slots[1].weapon = item;

			StorageCharacter davidCharacter = PersistenceManager.Instance.GetStorageCharacter("David");
			davidCharacter.AddItem(ItemRepository.Instance.GetExactItem("Call Out"));
		}

		if (currentLevel == 3)
		{
			StorageCharacter lenetteCharacter = PersistenceManager.Instance.GetStorageCharacter("Cotton");
			lenetteCharacter.AddItem(ItemRepository.Instance.GetExactItem("Medkit"));

			StorageCharacter davidCharacter = PersistenceManager.Instance.GetStorageCharacter("Dane");
			davidCharacter.AddItem(ItemRepository.Instance.GetExactItem("Curse Paper"));

			PersistenceManager.Instance.SetFlag("WeaponSlotsLocked", true);
			PersistenceManager.Instance.SetFlag("AdvantagesLocked", true);
			PersistenceManager.Instance.SetFlag("EnergyLocked", true);

			PersistenceManager.Instance.AddUnlockedItem("Amber Spyglass");
			PersistenceManager.Instance.AddUnlockedItem("Causality Gem");
			PersistenceManager.Instance.AddUnlockedItem("Shield");
			PersistenceManager.Instance.AddUnlockedItem("Call Out");
			PersistenceManager.Instance.AddUnlockedItem("Medkit");
			PersistenceManager.Instance.AddUnlockedItem("Curse Paper");
		}
	}

	//-------------------------------------------
	
	public void CompleteForest()
	{
		TownSystemPopup.QueueMessage("You've unlocked the ability to change weapons, and many new weapons.");
		PersistenceManager.Instance.SetFlag("WeaponSlotsLocked", false);
		//give weapons
		PersistenceManager.Instance.AddUnlockedItem("Phantasm");
		PersistenceManager.Instance.AddUnlockedItem("Rapier");
		PersistenceManager.Instance.AddUnlockedItem("Sparks");
		PersistenceManager.Instance.AddUnlockedItem("Healing Spell");
		PersistenceManager.Instance.AddUnlockedItem("Spear");
		PersistenceManager.Instance.AddUnlockedItem("Big Sword");
		PersistenceManager.Instance.AddUnlockedItem("Great Club");

		PersistenceManager.Instance.SetFlag("ForestComplete", true);
	}

	public void CompleteMeadow()
	{
		TownSystemPopup.QueueMessage("You've unlocked energy!  Use it to surge and use powerful abilities.");
		PersistenceManager.Instance.SetFlag("EnergyLocked", false);
		//give weapons
		PersistenceManager.Instance.AddUnlockedItem("Fireball");
		PersistenceManager.Instance.AddUnlockedItem("Lightning Bolt");
		PersistenceManager.Instance.SetFlag("MeadowComplete", true);
	}

	public void CompleteWoods()
	{
		TownSystemPopup.QueueMessage("You've unlocked advantages!  You can now change your characters advantrages in town.");
		PersistenceManager.Instance.SetFlag("AdvantagesLocked", false);
		
		//give advantages
		PersistenceManager.Instance.AddUnlockedItem("Protect");
		PersistenceManager.Instance.AddUnlockedItem("Energy Potion");
		PersistenceManager.Instance.AddUnlockedItem("Health Potion");
		PersistenceManager.Instance.SetFlag("WoodsComplete", true);
	}

	/////////////////////////////////////////////////////
	
	public bool SetIsLocked(string functionName)
	{
		if (string.IsNullOrEmpty(functionName))
		{
			return false;
		}
		System.Object[] paramsList = new System.Object[]
		{
		};
		return (bool)this.GetType().GetMethod(functionName).Invoke(this, paramsList);
	}

	public bool SetIsComplete(string functionName)
	{
		if (string.IsNullOrEmpty(functionName))
		{
			return false;
		}
		System.Object[] paramsList = new System.Object[]
		{
		};
		return (bool)this.GetType().GetMethod(functionName).Invoke(this, paramsList);
	}


	/////////////////////////////////////////////////////

	
	public bool MeadowIsLocked()
	{
		return PersistenceManager.Instance.GetFlag("WeaponSlotsLocked");
	}

	public bool WoodsIsLocked()
	{
		return PersistenceManager.Instance.GetFlag("WeaponSlotsLocked");
	}

	public bool BossIsLocked()
	{
		return MeadowIsComplete() == false ||
			WoodsIsComplete() == false;
	}

	/////////////////////////////////////////////////////
	
	public bool MeadowIsComplete()
	{
		return PersistenceManager.Instance.GetFlag("MeadowComplete");
	}

	public bool ForestIsComplete()
	{
		return PersistenceManager.Instance.GetFlag("ForestComplete");
	}


	public bool WoodsIsComplete()
	{
		return PersistenceManager.Instance.GetFlag("WoodsComplete");
	}

	/////////////////////////////////////////////////////
}
