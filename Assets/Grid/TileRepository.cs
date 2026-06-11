using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileRepository : Singleton<TileRepository>
{
	TileCollection tilesReal;
	Dictionary<string, Tile> tiles;

	private void Awake()
	{
		tilesReal = Resources.Load<GameObject>("TileCollection").GetComponent<TileCollection>();
		tiles = new Dictionary<string, Tile>();
		foreach (Tile tile in tilesReal.tiles)
		{
			tiles[tile.name] = tile;
		}
	}

	public Tile GetExactTile(string tileName)
	{
		return tiles[tileName];
	}
}
