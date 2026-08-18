using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class LevelEditorSaveManager : MonoBehaviour
{
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
		public string swatchName;
		public string tileStateScriptableObjectName;
		public SavedFallThroughObjectData fallThroughObjectData;
		public int seed;
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

		foreach (Transform propTransform in LevelEditorManager.Instance.propsParent.transform)
		{
			map.props.Add(FormProp(propTransform));
		}

		return map;
	}

	SavedProp FormProp(Transform propTransform)
	{
		SavedProp prop = new SavedProp();
		prop.position = propTransform.localPosition;
		prop.rotation = propTransform.localEulerAngles;
		prop.scale = propTransform.localScale;
		prop.propName = CleanPropName(propTransform.name);
		prop.parent = false;
		prop.group = -1;
		return prop;
	}

	string CleanPropName(string propName)
	{
		string cleaned = propName.Replace("(Clone)", "");
		cleaned = Regex.Replace(cleaned, @"\s*\(\d+\)$", "");
		cleaned = Regex.Replace(cleaned, @"\s*\d+$", "");
		return cleaned.Trim();
	}

	SavedMapTile FormMapTile(EmptyTile emptyTile)
	{
		SavedMapTile mapTile = new SavedMapTile();
		mapTile.x = emptyTile.x;
		mapTile.y = emptyTile.y;
		mapTile.swatchName = emptyTile.swatch.name;
		mapTile.tileStateScriptableObjectName = emptyTile.swatch.tileStateScriptableObjectName;
		mapTile.seed = emptyTile.seed;
		mapTile.fallThroughObjectData = new SavedFallThroughObjectData();
		mapTile.fallThroughObjectData.enterable = true;
		mapTile.fallThroughObjectData.blocksLOS = false;
		mapTile.fallThroughObjectData.forceEnterable = false;

		return mapTile;
	}

	public void ClickSave()
	{
		if (!Save(mapName.text))
		{
			return;
		}
		Debug.Log("Save Complete");
	}

	public bool Save(string fileName)
	{
		if (fileName == "")
		{
			Debug.LogError("Cannot save a map with an empty name");
			return false;
		}

		try
		{
			File.WriteAllText($"Assets/Resources/Maps/{fileName}.txt", Util.JSONSerializer.Serialize(FormMap()));
		}
		catch (System.Exception e)
		{
			Debug.LogError($"Failed to save map {fileName}: {e.Message}");
			return false;
		}

		return true;
	}

	public void ClickLoad()
	{
#if UNITY_EDITOR
		string path = EditorUtility.OpenFilePanel("Load Map", "Assets/Resources/Maps", "txt");
		if (path == "")
		{
			return;
		}

		Load(path);
		Debug.Log($"Load Complete: {path}");
#endif
	}

	public void Load(string fullPath)
	{
		SavedMap map = Util.JSONSerializer.Deserialize<SavedMap>(File.ReadAllText(fullPath));

		LevelEditorManager.Instance.Generate(map.width, map.height);
		mapName.text = Path.GetFileNameWithoutExtension(fullPath);

		foreach (SavedMapTile mapTile in map.mapTiles)
		{
			int index = mapTile.x + mapTile.y * map.width;

			SwatchScriptableObject swatch = FindSwatch(mapTile.swatchName);

			EmptyTile emptyTile = LevelEditorManager.Instance.tiles[index].GetComponent<EmptyTile>();
			emptyTile.seed = mapTile.seed;
			emptyTile.ChangeToTile(swatch, false);
		}

		LoadProps(map);
	}

	void LoadProps(SavedMap map)
	{
		Transform propsParent = LevelEditorManager.Instance.propsParent.transform;

		for (int i = propsParent.childCount - 1; i >= 0; --i)
		{
			Destroy(propsParent.GetChild(i).gameObject);
		}

		foreach (SavedProp prop in map.props)
		{
			GameObject obj = Instantiate(PropRepository.Instance.GetProp(prop.propName));
			obj.name = prop.propName;
			obj.transform.SetParent(propsParent);
			obj.transform.localPosition = prop.position;
			obj.transform.localEulerAngles = prop.rotation;
			obj.transform.localScale = prop.scale;
		}
	}

	SwatchScriptableObject FindSwatch(string swatchName)
	{
		foreach (SwatchScriptableObject swatch in SwatchRepository.Instance.GetSwatches())
		{
			if (swatch.name == swatchName)
			{
				return swatch;
			}
		}
		return null;
	}
}
