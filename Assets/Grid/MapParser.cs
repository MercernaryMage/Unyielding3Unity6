using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MapParser
{
	[System.Serializable]
	public class Map
	{
		public int width;
		public int height;
		public List<MapTile> mapTiles;
		public List<Prop> props;
	}

	[System.Serializable]
	public class MapTile
	{
		public int x;
		public int y;
		public string swatchName;
		public string tileStateScriptableObjectName;
	}

	[System.Serializable]
	public class Prop
	{
		public Vector3 position;
		public Vector3 rotation;
		public Vector3 scale;
		public string propName;
	}

	static public Map ParseMap(string file)
	{
		if (file == "")
		{
			Debug.LogError("File param is empty string");
		}
		string str = File.ReadAllText($"Assets/Resources/Maps/{file}");
		Map map =  JsonUtility.FromJson<Map>(str);
		return map;
	}
}
