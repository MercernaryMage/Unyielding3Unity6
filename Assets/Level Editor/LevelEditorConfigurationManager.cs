using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class LevelEditorConfigurationManager : MonoBehaviour
{
	public int currentPlayerIndex = -1;
	public string currentMonsterName = "";
	List<MapLocation> playerPositions = new List<MapLocation>()
	{
		new MapLocation(0,0,0),
		new MapLocation(0,0,0),
		new MapLocation(0,0,0),
		new MapLocation(0,0,0)
	};
	List<NPCLocation> enemyConfigurations = new List<NPCLocation>();

	Dictionary<EmptyTile, string> enemyTiles = new Dictionary<EmptyTile, string>();

	List<GameObject> createdObjects = new List<GameObject>();
	public GameObject monsterButtonPrefab;
	public Transform monsterButtonTarget;

	public List<GameObject> playerModels;

	int pageNumber;

	public void Start()
	{
		ShowMonsterList();
	}

	public void SetPlayerNumber(int playerNumber)
	{
		currentPlayerIndex = playerNumber;
	}

	public bool ClickTile(EmptyTile t)
	{
		if (currentPlayerIndex != -1)
		{
			t.playerIndex = currentPlayerIndex;
			playerPositions[currentPlayerIndex] = new MapLocation(t.x, t.y, 0);
			GameObject playerModel = Instantiate(playerModels[currentPlayerIndex]);
			playerModel.transform.SetParent(t.transform, false);
			currentPlayerIndex = -1;
			return true;
		}
		else if (currentMonsterName != "")
		{
			t.mosnterName = currentMonsterName;
			enemyConfigurations.Add(new NPCLocation(currentMonsterName, t.x, t.y, 0));
			GameObject monsterModel = Instantiate(CharacterRepository.Instance.GetCharacter(currentMonsterName).model);
			monsterModel.transform.SetParent(t.transform, false);

			currentMonsterName = "";

			enemyTiles[t] = currentMonsterName;

			return true;
		}
		return false;
	}

	public void ShowMonsterList()
	{
		foreach (GameObject obj in createdObjects) 
		{
			Destroy(obj);
		}
		createdObjects.Clear();

		if (pageNumber < 0)
		{
			return;
		}

		List<CharacterScriptableObject> enemies = CharacterRepository.Instance.GetAllEnemies();

		for (int i = 0; i < 6 && i + 6 * pageNumber < enemies.Count; ++i)
		{
			CharacterScriptableObject characterScriptableObject = enemies[i + pageNumber * 6];
			GameObject obj = Instantiate(monsterButtonPrefab);
			obj.transform.SetParent(monsterButtonTarget);
			obj.GetComponent<MonsterPickerButton>().Set(characterScriptableObject.name, this);
			createdObjects.Add(obj);
		}

	}

	public void IncreasePageNumber()
	{
		++pageNumber;
		ShowMonsterList();
	}

	public void DecreasePageNumber()
	{
		--pageNumber;
		ShowMonsterList();
	}

	public void MonsterButtonClicked(string s)
	{
		currentMonsterName = s;
	}

	public void ConfigurationCancelClicked()
	{
		currentMonsterName = "";
	}

	public void SaveClicked()
	{
#if UNITY_EDITOR
		string path = EditorUtility.SaveFilePanel("Save Config", "Assets/Resources/MapSets/Configurations", "newConfiguration","txt");

		MapConfiguration mapConfiguration = new MapConfiguration();
		mapConfiguration.enemies = enemyConfigurations;
		mapConfiguration.player = playerPositions;

		File.WriteAllText(path, Util.JSONSerializer.Serialize(mapConfiguration));
#endif
	}

	public void LoadClicked()
	{
#if UNITY_EDITOR
		string path = EditorUtility.OpenFilePanel("Load Config", "Assets/Resources/MapSets/Configurations", "txt");
#endif
	}
}
