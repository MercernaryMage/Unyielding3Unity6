using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScriptableObjectRepository : Singleton<TileScriptableObjectRepository>
{
	TileScriptableObjectCollection tileScriptableObjectsReal;
	Dictionary<string, TileScriptableObject> tileScriptableObjects;

	private void Awake()
	{
		tileScriptableObjectsReal = Resources.Load<GameObject>("TileScriptableObjectCollection").GetComponent<TileScriptableObjectCollection>();
		tileScriptableObjects = new Dictionary<string, TileScriptableObject>();
		foreach (TileScriptableObject tile in tileScriptableObjectsReal.tiles)
		{
			tileScriptableObjects[tile.name] = tile;
		}
	}

	public TileScriptableObject GetExactTile(string tileName)
	{
		return tileScriptableObjects[tileName];
	}
}
