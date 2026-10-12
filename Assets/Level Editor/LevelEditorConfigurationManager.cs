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

	class PlacedCharacter
	{
		public GameObject model;
		public List<EmptyTile> footprint = new List<EmptyTile>();
		public int playerIndex = -1;
		public string monsterName = "";
		public int direction;
	}

	List<PlacedCharacter> placedCharacters = new List<PlacedCharacter>();

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
			PlacePlayer(t, currentPlayerIndex, 0);
			currentPlayerIndex = -1;
			return true;
		}
		else if (currentMonsterName != "")
		{
			if (!PlaceMonster(t, currentMonsterName, 0))
			{
				return false;
			}

			enemyConfigurations.Add(new NPCLocation(currentMonsterName, t.x, t.y, 0));

			currentMonsterName = "";

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
		if (path == "")
		{
			return;
		}

		Load(path);
#endif
	}

	public void Load(string fullPath)
	{
		MapConfiguration mapConfiguration = Util.JSONSerializer.Deserialize<MapConfiguration>(File.ReadAllText(fullPath));

		ClearConfiguration();

		for (int i = 0; i < playerPositions.Count && i < mapConfiguration.player.Count; ++i)
		{
			MapLocation playerLocation = mapConfiguration.player[i];
			playerPositions[i] = playerLocation;

			EmptyTile emptyTile = FindTile(playerLocation.x, playerLocation.y);
			if (emptyTile == null)
			{
				Debug.LogWarning($"No tile at {playerLocation.x}, {playerLocation.y} for player {i}");
				continue;
			}

			emptyTile.playerIndex = i;
			PlacePlayer(emptyTile, i, playerLocation.direction);
		}

		foreach (NPCLocation enemy in mapConfiguration.enemies)
		{
			CharacterScriptableObject characterScriptableObject = CharacterRepository.Instance.GetCharacter(enemy.name);
			if (characterScriptableObject == null)
			{
				continue;
			}

			enemyConfigurations.Add(enemy);

			EmptyTile emptyTile = FindTile(enemy.location.x, enemy.location.y);
			if (emptyTile == null)
			{
				Debug.LogWarning($"No tile at {enemy.location.x}, {enemy.location.y} for {enemy.name}");
				continue;
			}

			PlaceMonster(emptyTile, enemy.name, enemy.location.direction);
		}
	}

	void ClearConfiguration()
	{
		foreach (PlacedCharacter placedCharacter in placedCharacters)
		{
			placedCharacter.model.transform.SetParent(null);
			Destroy(placedCharacter.model);
		}
		placedCharacters.Clear();

		foreach (GameObject tileObject in LevelEditorManager.Instance.tiles)
		{
			EmptyTile emptyTile = tileObject.GetComponent<EmptyTile>();
			emptyTile.playerIndex = -1;
			emptyTile.mosnterName = "";
		}

		for (int i = 0; i < playerPositions.Count; ++i)
		{
			playerPositions[i] = new MapLocation(0, 0, 0);
		}

		enemyConfigurations.Clear();
		enemyTiles.Clear();

		currentPlayerIndex = -1;
		currentMonsterName = "";
	}

	bool PlaceMonster(EmptyTile emptyTile, string monsterName, int direction)
	{
		CharacterScriptableObject characterScriptableObject = CharacterRepository.Instance.GetCharacter(monsterName);
		if (characterScriptableObject == null)
		{
			return false;
		}

		List<EmptyTile> footprint = GetFootprint(emptyTile, characterScriptableObject.size);
		if (footprint == null)
		{
			Debug.LogWarning($"{monsterName} does not fit at {emptyTile.x}, {emptyTile.y}");
			return false;
		}

		foreach (EmptyTile footprintTile in footprint)
		{
			footprintTile.mosnterName = monsterName;
			enemyTiles[footprintTile] = monsterName;
		}

		PlacedCharacter placedCharacter = AddCharacterModel(footprint, characterScriptableObject.model,
			characterScriptableObject.size, direction);
		placedCharacter.monsterName = monsterName;
		return true;
	}

	void PlacePlayer(EmptyTile emptyTile, int playerIndex, int direction)
	{
		PlacedCharacter placedCharacter = AddCharacterModel(new List<EmptyTile>() { emptyTile },
			playerModels[playerIndex], 1, direction);
		placedCharacter.playerIndex = playerIndex;
	}

	public bool RotateCharacter(EmptyTile emptyTile)
	{
		PlacedCharacter placedCharacter = FindPlacedCharacter(emptyTile);
		if (placedCharacter == null)
		{
			return false;
		}

		placedCharacter.direction = (placedCharacter.direction + 1) % 4;
		placedCharacter.model.transform.localRotation = Util.GetFacing((Direction)placedCharacter.direction);

		foreach (EmptyTile footprintTile in placedCharacter.footprint)
		{
			footprintTile.direction = (Direction)placedCharacter.direction;
		}

		if (placedCharacter.playerIndex != -1)
		{
			playerPositions[placedCharacter.playerIndex].direction = placedCharacter.direction;
			return true;
		}

		NPCLocation enemyConfiguration = FindEnemyConfiguration(placedCharacter);
		if (enemyConfiguration != null)
		{
			enemyConfiguration.location.direction = placedCharacter.direction;
		}
		return true;
	}

	public bool DeleteCharacter(EmptyTile emptyTile)
	{
		PlacedCharacter placedCharacter = FindPlacedCharacter(emptyTile);
		if (placedCharacter == null)
		{
			return false;
		}

		foreach (EmptyTile footprintTile in placedCharacter.footprint)
		{
			if (placedCharacter.playerIndex != -1)
			{
				footprintTile.playerIndex = -1;
			}
			else
			{
				footprintTile.mosnterName = "";
				enemyTiles.Remove(footprintTile);
			}
		}

		if (placedCharacter.playerIndex != -1)
		{
			playerPositions[placedCharacter.playerIndex] = new MapLocation(0, 0, 0);
		}
		else
		{
			NPCLocation enemyConfiguration = FindEnemyConfiguration(placedCharacter);
			if (enemyConfiguration != null)
			{
				enemyConfigurations.Remove(enemyConfiguration);
			}
		}

		placedCharacter.model.transform.SetParent(null);
		Destroy(placedCharacter.model);
		placedCharacters.Remove(placedCharacter);
		return true;
	}

	PlacedCharacter FindPlacedCharacter(EmptyTile emptyTile)
	{
		foreach (PlacedCharacter placedCharacter in placedCharacters)
		{
			if (placedCharacter.footprint.Contains(emptyTile))
			{
				return placedCharacter;
			}
		}
		return null;
	}

	NPCLocation FindEnemyConfiguration(PlacedCharacter placedCharacter)
	{
		EmptyTile anchorTile = placedCharacter.footprint[0];
		foreach (NPCLocation enemy in enemyConfigurations)
		{
			if (enemy.name == placedCharacter.monsterName &&
				enemy.location.x == anchorTile.x &&
				enemy.location.y == anchorTile.y)
			{
				return enemy;
			}
		}
		return null;
	}

	List<EmptyTile> GetFootprint(EmptyTile emptyTile, int size)
	{
		List<EmptyTile> footprint = new List<EmptyTile>();
		for (int y = 0; y < size; ++y)
		{
			for (int x = 0; x < size; ++x)
			{
				EmptyTile footprintTile = FindTile(emptyTile.x + x, emptyTile.y + y);
				if (footprintTile == null)
				{
					return null;
				}
				footprint.Add(footprintTile);
			}
		}
		return footprint;
	}

	PlacedCharacter AddCharacterModel(List<EmptyTile> footprint, GameObject model, int size, int direction)
	{
		GameObject obj = Instantiate(model);
		obj.transform.SetParent(footprint[0].transform, false);
		obj.transform.localScale = Vector3.one * size;
		obj.transform.localRotation = Util.GetFacing((Direction)direction);

		Vector3 center = Vector3.zero;
		foreach (EmptyTile footprintTile in footprint)
		{
			center += footprintTile.transform.position;
		}
		obj.transform.position = center / footprint.Count;

		PlacedCharacter placedCharacter = new PlacedCharacter();
		placedCharacter.model = obj;
		placedCharacter.footprint = footprint;
		placedCharacter.direction = direction;
		placedCharacters.Add(placedCharacter);

		return placedCharacter;
	}

	EmptyTile FindTile(int x, int y)
	{
		foreach (GameObject tileObject in LevelEditorManager.Instance.tiles)
		{
			EmptyTile emptyTile = tileObject.GetComponent<EmptyTile>();
			if (emptyTile.x == x && emptyTile.y == y)
			{
				return emptyTile;
			}
		}
		return null;
	}
}
