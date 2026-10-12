using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CharacterRepository : Singleton<CharacterRepository>
{
	public CharacterRepositoryData data;
	public void Awake()
	{
		data = Resources.Load<CharacterRepositoryData>("CharacterRepositoryData");
	}

	public CharacterScriptableObject GetCharacter(string characterName)
	{
		foreach (CharacterScriptableObject character in data.characters)
		{
			if (character.name == characterName)
			{
				return character;
			}
		}
		Debug.LogError($"{characterName} not found in the Character Repository");
		return null;
	}

	public List<CharacterScriptableObject> GetAllEnemies()
	{
		return data.characters.Where(o => o.enemy).ToList();
	}
}
