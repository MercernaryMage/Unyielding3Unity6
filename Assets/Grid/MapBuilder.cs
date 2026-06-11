using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;
using static MapParser;
using static Util;

public class MapBuilder : MonoBehaviour
{
	public GameObject root;
	public GameObject tilePrefab;
	public GameObject overlayPrefab;
	public int startLevel = 0;
	public Transform props;
	public GameObject foundationPrefab;

	static bool needsInit = true;
	static Dictionary<string, int> enemyNameCounts = new Dictionary<string, int>();

	private void Awake()
	{
		if (Application.isEditor)
		{

			if (!needsInit)
			{
				return;
			}
			needsInit = false;
			if (!DebugStartup.init)
			{
				FlowControl.currentLevel = startLevel;
			}

		}
	}

	void PlaceCharacters()
	{
		enemyNameCounts.Clear();
		LevelConfiguration levelConfiguration = GetLevelConfiguration(FlowControl.currentLevel);

		for (int i = 0; i < levelConfiguration.players.Count; ++i)
		{
			PlaceHero(levelConfiguration.players[i].x,
				levelConfiguration.players[i].y,
				PersistenceManager.Instance.currentTeam[i],
				(Direction)levelConfiguration.players[i].facing);
		}

		for (int i = 0; i < levelConfiguration.enemies.Count; ++i)
		{
			PlaceEnemy(
				levelConfiguration.enemies[i].positionConfiguration.x,
				levelConfiguration.enemies[i].positionConfiguration.y,
				CharacterRepository.Instance.GetCharacter(levelConfiguration.enemies[i].enemyName),
				(Direction)levelConfiguration.enemies[i].positionConfiguration.facing);
		}

		//PlaceCharacter(5, 4, CharacterRepository.Instance.GetCharacter("BigTestEnemy"), Direction.North, false);
		//PlaceCharacter(6, 4, CharacterRepository.Instance.GetCharacter("Bandit"), Direction.North, false);
		//PlaceCharacter(1, 4, CharacterRepository.Instance.GetCharacter("Guard"), Direction.North, false);



		List<Character> characters = new List<Character>();
		characters.AddRange(BattleController.Instance.heroes);
		characters.AddRange(BattleController.Instance.enemies);
		TurnControl.Instance.AddCharacters(characters);

		BattleController.playerHasControl = true;
		BattleController.Instance.RequestRunDuckers();
	}

	static void HandleTraits(Character character)
	{
		if (PersistenceManager.Instance.GetFlag("WeaponSlotsLocked"))
		{
			return;
		}

		foreach (TraitScriptableObject traitScriptableObject in character.characterDefinition.traits)
		{
			Trait t = (Trait)character.gameObject.AddComponent(Type.GetType(traitScriptableObject.className));
			t.scriptableObject = traitScriptableObject;
		}
	}

	public static Character PlaceHero(int x, int y, StorageCharacter storageCharacter, Direction facing)
	{
		foreach (Item item in storageCharacter.equipment)
		{
			item.loaded = true;
		}

		GameObject characterObj = new GameObject();
		Character character = characterObj.AddComponent<Character>();
		storageCharacter.createdCharacter = character;
		character.storageCharacter = storageCharacter;

		character.hero = true;
		character.Init(storageCharacter.characterDefintion);
		character.gameObject.name = character.displayName;
		character.SetFacing(facing);
		TileGrid.Instance.PlaceCharacter(x, y, character);

		AttackOfOpportunityTrait attackOfOpportunityTrait = characterObj.AddComponent<AttackOfOpportunityTrait>();

		bool hasLoadingWeapon = false;
		if (PersistenceManager.Instance.GetFlag("EnergyLocked"))
		{
			foreach (Item item in storageCharacter.equipment)
			{
				if (item.itemDefinition == null) continue;
				foreach (ActionPattern action in item.itemDefinition.actions)
				{
					if (action.keywords != null && action.keywords.ContainsIgnoreCase("Loading"))
					{
						hasLoadingWeapon = true;
						break;
					}
				}
				if (hasLoadingWeapon) break;
			}
		}

		if (hasLoadingWeapon || !PersistenceManager.Instance.GetFlag("EnergyLocked"))
		{
			character.temporaryItems.Add(Item.CreateItem(ItemRepository.Instance.GetExactItem("Ready")));
		}
		if (!PersistenceManager.Instance.GetFlag("EnergyLocked"))
		{
			character.temporaryItems.Add(Item.CreateItem(ItemRepository.Instance.GetExactItem("Surge")));
			character.temporaryItems.Add(Item.CreateItem(ItemRepository.Instance.GetExactItem("Dash")));
		}
		HandleTraits(character);
		return character;
	}

	public static Character PlaceEnemy(int x, int y, CharacterScriptableObject characterScriptableObject, Direction facing)
	{
		GameObject characterObj = new GameObject();
		Character character = characterObj.AddComponent<Character>();

		character.hero = false;
		character.Init(characterScriptableObject);
		string baseName = character.displayName;
		if (!enemyNameCounts.ContainsKey(baseName))
		{
			enemyNameCounts[baseName] = 0;
		}
		enemyNameCounts[baseName]++;
		character.displayName = $"{baseName} {enemyNameCounts[baseName]}";
		character.displayNumber = enemyNameCounts[baseName];
		character.gameObject.name = character.displayName;

		characterObj.AddComponent<AttackOfOpportunityTrait>();

		character.SetFacing(facing);
		TileGrid.Instance.PlaceCharacter(x, y, character);
		HandleTraits(character);
		return character;
	}

	void Start()
	{
		Run();
	}

	void Run()
	{
		LevelConfiguration levelConfiguration = GetLevelConfiguration(FlowControl.currentLevel);
		MapSetScriptableObject mapSet = Resources.Load<MapSetScriptableObject>($"MapSets/{FlowControl.mapSetName}");
		MapParser.Map map;
		if (FlowControl.currentLevel >= 0)
		{
			map = MapParser.ParseMap(mapSet.maps[FlowControl.currentLevel]);
		}
		else
		{
			map = MapParser.ParseMap("TestMap4.txt");
		}
		float tileScale = 1.5f;
		List<Tile> tiles = new List<Tile>();
		for (int y = 0; y < map.height; ++y)
		{
			for (int x = 0; x < map.width; ++x)
			{
				GameObject obj = new GameObject();
				obj.name = $"{x}, {y}";
				obj.transform.SetParent(root.transform);
				obj.transform.localPosition = new Vector3(x * (tileScale + .75f), 0, y * (tileScale + .75f));

				MapTile mapTile = map.mapTiles[x + y * map.height];
				string prefabName = mapTile.mainDeco.decoPrefab;
				GameObject tileObj = Instantiate(TileRepository.Instance.GetExactTile(prefabName).gameObject);
				Tile t = tileObj.GetComponent<Tile>();
				t.name = $"{x}, {y} tile";

				TileScriptableObject tileScriptableObject = TileScriptableObjectRepository.Instance.GetExactTile(mapTile.tileStateScriptableObjectName);
				t.tileScriptableObject = tileScriptableObject;

				tileObj.transform.SetParent(obj.transform);
				tileObj.transform.localScale = new Vector3(tileScale, .1f, tileScale);
				tileObj.transform.localPosition = Vector3.zero;

				if (mapTile.subDeco.decoPrefab != "")
				{
					Deco deco = mapTile.subDeco;

					GameObject subDeco = Instantiate(PropRepository.Instance.GetProp(deco.decoPrefab));
					subDeco.transform.SetParent(tileObj.transform);
					subDeco.transform.localPosition = Vector3.zero;
					subDeco.transform.eulerAngles = deco.rotation;
					t.decos.Add(subDeco);
				}

				GameObject overlay = Instantiate(overlayPrefab);
				overlay.transform.SetParent(obj.transform);
				overlay.transform.localScale = new Vector3(tileScale + .75f, tileScale + .75f, 1);
				overlay.transform.localPosition = new Vector3(0, .03f, 0);

				GameObject foundation = Instantiate(foundationPrefab);
				foundation.transform.SetParent(tileObj.transform);
				foundation.transform.localPosition = new Vector3(0, -5.0f, 0);
				foundation.transform.localScale = new Vector3(1.5f, 10, 1.5f);
				t.foundation = foundation;

				t.Init(x, y, tileObj, overlay);
				tiles.Add(t);
			}
		}

		HandleSkirt(tiles, map);

		foreach (MapParser.Prop prop in map.props)
		{
			GameObject obj = Instantiate(PropRepository.Instance.GetProp(prop.propName));
			obj.transform.SetParent(props);
			obj.transform.localPosition = prop.position;
			obj.transform.eulerAngles = prop.rotation;
			obj.transform.localScale = prop.scale;
		}

		TileGrid.Instance.Init(map.width, map.height, tiles);
		PlaceCharacters();
	}

	LevelConfiguration GetLevelConfiguration(int i)
	{
		LevelConfiguration levelConfiguration = new LevelConfiguration();
		MapSetScriptableObject mapSet = Resources.Load<MapSetScriptableObject>($"MapSets/{FlowControl.mapSetName}");

		if (i == -1)
		{
			levelConfiguration.players.Add(new PositionConfiguration(Direction.East, 2, 2));
			levelConfiguration.players.Add(new PositionConfiguration(Direction.East, 1, 5));
			levelConfiguration.players.Add(new PositionConfiguration(Direction.East, 2, 5));
			levelConfiguration.players.Add(new PositionConfiguration(Direction.East, 0, 2));

			//levelConfiguration.enemies.Add(new EnemyConfiguration("BigTestEnemy", Direction.West, 6, 5));
			levelConfiguration.enemies.Add(new EnemyConfiguration("Dragon", Direction.West, 6, 2));
			return levelConfiguration;
		}

		string path = mapSet.configurations[i];
		string data = Resources.Load<TextAsset>($"Mapsets/Configurations/{path}").text;
		MapConfiguration mapConfiguration = JSONSerializer.Deserialize<MapConfiguration>(data);
		foreach (MapLocation playerLocation in mapConfiguration.player)
		{
			levelConfiguration.players.Add(
				new PositionConfiguration((Direction)playerLocation.direction, playerLocation.x, playerLocation.y));
		}
		foreach (NPCLocation enemy in mapConfiguration.enemies)
		{
			levelConfiguration.enemies.Add(
				new EnemyConfiguration(enemy.name, (Direction)enemy.location.direction,
				enemy.location.x, enemy.location.y));
		}
		if (mapConfiguration.mapMessageBody != "")
		{
			CombatSystemMessage.Instance.Show(mapConfiguration.mapMessageTitle, mapConfiguration.mapMessageBody);
		}
		return levelConfiguration;
	}

	void HandleSkirt(List<Tile> tiles, Map map)
	{
		for (int y = 0; y < map.height; ++y)
		{
			for (int x = 0; x < map.width; ++x)
			{
				Tile t = tiles[x + y * map.width];
				bool south = false;
				bool north = false;
				bool west = false;
				bool east = false;
				//if edge tile
				if (y == 0)
				{
					south = true;
				}
				if (x == 0)
				{
					west = true;
				}
				if (y == map.height - 1)
				{
					north = true;
				}
				if (x == map.width - 1)
				{
					east = true;
				}

				if (!t.tileScriptableObject.empty)
				{
					if (y > 0 && tiles[x + (y - 1) * map.width].tileScriptableObject.empty)
					{
						south = true;
					}
					if (y < map.height - 1 && tiles[x + (y + 1) * map.width].tileScriptableObject.empty)
					{
						north = true;
					}
					if (x > 0 && tiles[x - 1 + y * map.width].tileScriptableObject.empty)
					{
						west = true;
					}
					if (x < map.width - 1 && tiles[x + 1 + y * map.width].tileScriptableObject.empty)
					{
						east = true;
					}
				}
				//if your neighbor is empty
				t.SetSkirt(north, south, west, east);
			}
		}
	}
}

public class MapLocation
{
	public int x;
	public int y;
	public int direction;
}

public class NPCLocation
{
	public MapLocation location;
	public string name;
}

public class MapConfiguration
{
	public List<MapLocation> player = new List<MapLocation>();
	public List<NPCLocation> enemies = new List<NPCLocation>();
	public string mapMessageTitle;
	public string mapMessageBody;
}