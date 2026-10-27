using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class LevelEditorConfigurationManager : MonoBehaviour
{
	public string currentMonsterName = "";
	List<NPCLocation> enemyConfigurations = new List<NPCLocation>();

	Dictionary<EmptyTile, string> enemyTiles = new Dictionary<EmptyTile, string>();

	class PlacedCharacter
	{
		public GameObject model;
		public List<EmptyTile> footprint = new List<EmptyTile>();
		public string monsterName = "";
		public int direction;
	}

	List<PlacedCharacter> placedCharacters = new List<PlacedCharacter>();

	class PlacementTile
	{
		public EmptyTile tile;
		public GameObject marker;
		public Quaternion markerRotation;
		public GameObject model;
		public int direction;
	}

	List<PlacementTile> placementTiles = new List<PlacementTile>();

	List<GameObject> createdObjects = new List<GameObject>();
	public GameObject monsterButtonPrefab;
	public Transform monsterButtonTarget;

	public List<GameObject> playerModels;

	string preFightFunc;

	int pageNumber;

	public void Start()
	{
		ShowMonsterList();
	}

	public bool ClickTile(EmptyTile t)
	{
		if (currentMonsterName != "")
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
		mapConfiguration.player = GetPlayerLocations();
		mapConfiguration.preFightFunc = preFightFunc;

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

		preFightFunc = mapConfiguration.preFightFunc;

		for (int i = 0; i < mapConfiguration.player.Count; ++i)
		{
			MapLocation playerLocation = mapConfiguration.player[i];

			EmptyTile emptyTile = FindTile(playerLocation.x, playerLocation.y);
			if (emptyTile == null)
			{
				Debug.LogWarning($"No tile at {playerLocation.x}, {playerLocation.y} for player {i}");
				continue;
			}

			AddPlacementTile(emptyTile, playerLocation.direction);
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

		ClearPlacementTiles();

		enemyConfigurations.Clear();
		enemyTiles.Clear();

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

	public bool AddPlacementTile(EmptyTile emptyTile)
	{
		return AddPlacementTile(emptyTile, 0);
	}

	public bool AddPlacementTile(EmptyTile emptyTile, int direction)
	{
		if (emptyTile == null || IsPlacementTile(emptyTile))
		{
			return false;
		}

		PlacementTile placementTile = new PlacementTile();
		placementTile.tile = emptyTile;
		placementTile.direction = direction;
		placementTile.marker = Instantiate(LevelEditorManager.Instance.placementTilePrefab);
		placementTile.marker.transform.SetParent(emptyTile.transform.parent, false);
		placementTile.marker.transform.position = emptyTile.transform.position + new Vector3(0, .01f, 0);
		placementTile.markerRotation = placementTile.marker.transform.localRotation;

		placementTiles.Add(placementTile);
		RefreshPlacementModels();

		return true;
	}

	public bool RemovePlacementTile(EmptyTile emptyTile)
	{
		PlacementTile placementTile = FindPlacementTile(emptyTile);
		if (placementTile == null)
		{
			return false;
		}

		DestroyPlacementTile(placementTile);
		placementTiles.Remove(placementTile);
		RefreshPlacementModels();

		return true;
	}

	public bool IsPlacementTile(EmptyTile emptyTile)
	{
		return FindPlacementTile(emptyTile) != null;
	}

	public int GetPlacementIndex(EmptyTile emptyTile)
	{
		for (int i = 0; i < placementTiles.Count; ++i)
		{
			if (placementTiles[i].tile == emptyTile)
			{
				return i;
			}
		}
		return -1;
	}

	public bool RotatePlacementTile(EmptyTile emptyTile)
	{
		PlacementTile placementTile = FindPlacementTile(emptyTile);
		if (placementTile == null)
		{
			return false;
		}

		placementTile.direction = (placementTile.direction + 1) % 4;
		placementTile.tile.direction = (Direction)placementTile.direction;
		ApplyPlacementRotation(placementTile);

		return true;
	}

	void ApplyPlacementRotation(PlacementTile placementTile)
	{
		Quaternion facing = Util.GetFacing((Direction)placementTile.direction);

		if (placementTile.marker != null)
		{
			placementTile.marker.transform.localRotation = facing * placementTile.markerRotation;
		}
		if (placementTile.model != null)
		{
			placementTile.model.transform.localRotation = facing;
		}
	}

	PlacementTile FindPlacementTile(EmptyTile emptyTile)
	{
		foreach (PlacementTile placementTile in placementTiles)
		{
			if (placementTile.tile == emptyTile)
			{
				return placementTile;
			}
		}
		return null;
	}

	void ClearPlacementTiles()
	{
		foreach (PlacementTile placementTile in placementTiles)
		{
			DestroyPlacementTile(placementTile);
		}
		placementTiles.Clear();
	}

	void DestroyPlacementTile(PlacementTile placementTile)
	{
		if (placementTile.marker != null)
		{
			placementTile.marker.transform.SetParent(null);
			Destroy(placementTile.marker);
		}
		DestroyPlacementModel(placementTile);
	}

	void DestroyPlacementModel(PlacementTile placementTile)
	{
		if (placementTile.model == null)
		{
			return;
		}
		placementTile.model.transform.SetParent(null);
		Destroy(placementTile.model);
		placementTile.model = null;
	}

	void RefreshPlacementModels()
	{
		for (int i = 0; i < placementTiles.Count; ++i)
		{
			PlacementTile placementTile = placementTiles[i];
			DestroyPlacementModel(placementTile);

			if (i < playerModels.Count)
			{
				GameObject model = Instantiate(playerModels[i]);
				model.transform.SetParent(placementTile.tile.transform, false);
				placementTile.model = model;
			}

			ApplyPlacementRotation(placementTile);
		}
	}

	List<MapLocation> GetPlayerLocations()
	{
		List<MapLocation> playerLocations = new List<MapLocation>();
		foreach (PlacementTile placementTile in placementTiles)
		{
			playerLocations.Add(new MapLocation(placementTile.tile.x, placementTile.tile.y, placementTile.direction));
		}
		return playerLocations;
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
			footprintTile.mosnterName = "";
			enemyTiles.Remove(footprintTile);
		}

		NPCLocation enemyConfiguration = FindEnemyConfiguration(placedCharacter);
		if (enemyConfiguration != null)
		{
			enemyConfigurations.Remove(enemyConfiguration);
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
