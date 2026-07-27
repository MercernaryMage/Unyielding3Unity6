using System.Collections.Generic;
using UnityEngine;

public class OutlineManager : SceneSingleton<OutlineManager>
{
	class OutlineEntry
	{
		public int id;
		public List<Tile> tiles;
		public Color color;
	}

	List<OutlineEntry> entries = new List<OutlineEntry>();
	List<Tile> outlinedTiles = new List<Tile>();
	int nextId = 0;

	public int Create(List<Tile> tiles, Color color)
	{
		OutlineEntry entry = new OutlineEntry();
		entry.id = nextId++;
		entry.tiles = new List<Tile>(tiles);
		entry.color = color;
		entries.Add(entry);
		Render();
		return entry.id;
	}

	public void Destroy(int id)
	{
		for (int i = 0; i < entries.Count; ++i)
		{
			if (entries[i].id == id)
			{
				entries.RemoveAt(i);
				break;
			}
		}
		Render();
	}

	void Render()
	{
		foreach (Tile t in outlinedTiles)
		{
			t.ClearOutline();
		}
		outlinedTiles.Clear();

		//entries are stored in creation order, so iterating them lets later (more
		//recent) creates overwrite the winning entry for any conflicting tile.
		Dictionary<Tile, OutlineEntry> tileToEntry = new Dictionary<Tile, OutlineEntry>();
		foreach (OutlineEntry entry in entries)
		{
			foreach (Tile t in entry.tiles)
			{
				tileToEntry[t] = entry;
			}
		}

		Dictionary<OutlineEntry, HashSet<Tile>> entrySets = new Dictionary<OutlineEntry, HashSet<Tile>>();
		foreach (OutlineEntry entry in entries)
		{
			entrySets[entry] = new HashSet<Tile>(entry.tiles);
		}

		foreach (KeyValuePair<Tile, OutlineEntry> pair in tileToEntry)
		{
			Tile t = pair.Key;
			HashSet<Tile> outlineSet = entrySets[pair.Value];
			bool north = !outlineSet.Contains(TileGrid.Instance.GetTile(t.x, t.y + 1));
			bool south = !outlineSet.Contains(TileGrid.Instance.GetTile(t.x, t.y - 1));
			bool west = !outlineSet.Contains(TileGrid.Instance.GetTile(t.x - 1, t.y));
			bool east = !outlineSet.Contains(TileGrid.Instance.GetTile(t.x + 1, t.y));
			t.SetOutline(north, south, west, east, pair.Value.color);
			outlinedTiles.Add(t);
		}
	}
}
