using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugStartup : MonoBehaviour
{
	public static bool init = false;

	public bool applyOverride;
	public List<CharacterOverrideSetup> overrideSetups;
	public List<ItemScriptableObject> unlockedItems;

	private void Start()
	{
		if (init)
		{
			return;
		}
		init = true;		

		if (applyOverride)
		{
			StartTutorial.CreateCharacters(overrideSetups, unlockedItems);
		}
		else
		{
			PersistenceManager.Instance.currentTeam.Add(new StorageCharacter(CharacterRepository.Instance.GetCharacter("Lenette")));
			PersistenceManager.Instance.currentTeam.Add(new StorageCharacter(CharacterRepository.Instance.GetCharacter("David")));
			PersistenceManager.Instance.currentTeam.Add(new StorageCharacter(CharacterRepository.Instance.GetCharacter("Dane")));
			PersistenceManager.Instance.currentTeam.Add(new StorageCharacter(CharacterRepository.Instance.GetCharacter("Cotton")));
		}
	}

	
}
