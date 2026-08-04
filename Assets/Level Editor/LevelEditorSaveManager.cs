using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class LevelEditorSaveManager : MonoBehaviour
{
	[System.Serializable]
	public class SavedDeco
	{
		public string decoPrefab;
		public Vector3 rotation;
	}

	[System.Serializable]
	public class SavedFallThroughObjectData
	{
		public bool enterable;
		public bool blocksLOS;
		public bool forceEnterable;
	}

	[System.Serializable]
	public class SavedMapTile
	{
		public int x;
		public int y;
		public string tileStateScriptableObjectName;
		public SavedDeco mainDeco;
		public SavedDeco subDeco;
		public SavedFallThroughObjectData fallThroughObjectData;
	}

	[System.Serializable]
	public class SavedProp
	{
		public Vector3 position;
		public Vector3 rotation;
		public Vector3 scale;
		public string propName;
		public bool parent;
		public int group;
	}

	[System.Serializable]
	public class SavedHero
	{
		public int x;
		public int y;
		public int facing;
	}

	[System.Serializable]
	public class SavedMap
	{
		public int width;
		public int height;
		public bool useSpecialOffset;
		public Vector3 specialOffset;
		public string levelController;
		public string background;
		public List<SavedMapTile> mapTiles = new List<SavedMapTile>();
		public List<SavedProp> props = new List<SavedProp>();
		public List<SavedHero> heroes = new List<SavedHero>();
	}

	public TMP_InputField mapName;
	public string defaultTileState = "Floor";

	public SavedMap FormMap()
	{
		SavedMap map = new SavedMap();
		map.width = LevelEditorManager.Instance.mapWidth;
		map.height = LevelEditorManager.Instance.mapHeight;
		map.useSpecialOffset = false;
		map.specialOffset = Vector3.zero;
		map.levelController = "";
		map.background = "";

		foreach (GameObject tileObject in LevelEditorManager.Instance.tiles)
		{
			map.mapTiles.Add(FormMapTile(tileObject.GetComponent<EmptyTile>()));
		}

		return map;
	}

	SavedMapTile FormMapTile(EmptyTile emptyTile)
	{
		SavedMapTile mapTile = new SavedMapTile();
		mapTile.x = emptyTile.x;
		mapTile.y = emptyTile.y;
		mapTile.tileStateScriptableObjectName = defaultTileState;
		mapTile.mainDeco = FormDeco(emptyTile.swatch.prefab0);
		mapTile.subDeco = FormDeco(emptyTile.swatch.prefab1);
		mapTile.fallThroughObjectData = new SavedFallThroughObjectData();
		mapTile.fallThroughObjectData.enterable = true;
		mapTile.fallThroughObjectData.blocksLOS = false;
		mapTile.fallThroughObjectData.forceEnterable = false;

		return mapTile;
	}

	SavedDeco FormDeco(GameObject prefab)
	{
		SavedDeco deco = new SavedDeco();
		deco.decoPrefab = prefab == null ? null : prefab.name;
		deco.rotation = Vector3.zero;
		return deco;
	}

	public void ClickSave()
	{
		Save(mapName.text);
		Debug.Log("Save Complete");
	}

	public void Save(string fileName)
	{
		if (fileName == "")
		{
			Debug.LogError("Cannot save a map with an empty name");
			return;
		}

		File.WriteAllText($"Assets/Resources/Maps/{fileName}.txt", Util.JSONSerializer.Serialize(FormMap()));
	}
}
