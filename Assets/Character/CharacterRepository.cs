using System.Collections.Generic;
using UnityEngine;

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
}
