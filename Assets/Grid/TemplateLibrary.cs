using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TemplateLibrary : SceneSingleton<TemplateLibrary>
{
	public class TilesAndDirection
	{
		public TilesAndDirection(List<Tile> t, Direction d)
		{
			tiles = t;
			direction = d;
		}
		public List<Tile> tiles;
		public Direction direction;
	}

	List<Tile> GetCharacterEdges(Character character, Direction direction)
	{
		List<Tile> outTiles = new List<Tile>();
		int size = character.characterDefinition.size;
		List<Tile> fitTiles = TileGrid.Instance.FindCharacter(character);

		Tile newT;

		for (int j = 0; j < size; ++j)
		{
			if (direction == Direction.North)
			{
				newT = fitTiles[size * size - 1 - j];
			}
			else if (direction == Direction.East)
			{
				newT = fitTiles[(size - 1) + j * size];
			}
			else if (direction == Direction.South)
			{
				newT = fitTiles[j];
			}
			else
			{
				newT = fitTiles[j * size];
			}
			outTiles.Add(newT);
		}

		return outTiles;
	}

	TilesAndDirection GetLargestDirection(List<Tile>[] tilesGroups)
	{
		int largestIndex = 0;
		int largestCharacterCount = -1;
		for (int i = 0; i < tilesGroups.Length; ++i)
		{
			int currentCharacterCount = 0;
			foreach (Tile t in tilesGroups[i])
			{
				if (t.character != null && t.character.hero && !t.character.IsDowned())
				{
					++currentCharacterCount;
				}
			}
			if (largestCharacterCount < currentCharacterCount)
			{
				largestCharacterCount = currentCharacterCount;
				largestIndex = i;
			}
		}

		bool validTarget = false;
		foreach (Tile t in tilesGroups[largestIndex])
		{
			if (t.character && t.character.hero == true)
			{

				validTarget = true;
			}
		}
		if (validTarget)
		{
			List<Tile> filteredTiles = new List<Tile>(tilesGroups[largestIndex]);
			filteredTiles.RemoveAll(t => t.character != null && t.character.GetComponent<Stasis>() != null);
			return new TilesAndDirection(filteredTiles, (Direction)largestIndex);
		}
		else
		{
			return null;
		}
	}

	public List<Tile> GetNorthFacingTiles(List<Tile> characterTiles)
	{
		List<Tile> northTiles = new List<Tile>();
		int maxY = characterTiles.Max(t => t.y);
		foreach (Tile t in TileGrid.Instance.tiles)
		{
			if (t.y > maxY + 1)
			{
				northTiles.Add(t);
			}
		}
		return northTiles;
	}

	public List<Tile> GetEastFacingTiles(List<Tile> characterTiles)
	{
		List<Tile> eastTiles = new List<Tile>();
		int maxX = characterTiles.Max(t => t.x);
		foreach (Tile t in TileGrid.Instance.tiles)
		{
			if (t.x > maxX + 1)
			{
				eastTiles.Add(t);
			}
		}
		return eastTiles;
	}

	public List<Tile> GetSouthFacingTiles(List<Tile> characterTiles)
	{
		List<Tile> southTiles = new List<Tile>();
		int minY = characterTiles.Min(t => t.y);
		foreach (Tile t in TileGrid.Instance.tiles)
		{
			if (t.y < minY - 1)
			{
				southTiles.Add(t);
			}
		}
		return southTiles;
	}

	public List<Tile> GetWestFacingTiles(List<Tile> characterTiles)
	{
		List<Tile> westTiles = new List<Tile>();
		int minX = characterTiles.Min(t => t.x);
		foreach (Tile t in TileGrid.Instance.tiles)
		{
			if (t.x < minX - 1)
			{
				westTiles.Add(t);
			}
		}
		return westTiles;
	}

	public TilesAndDirection GetMostFacingDirection(Character c)
	{
		List<Tile> tiles = TileGrid.Instance.FindCharacter(c);

		List<Tile>[] groups = new List<Tile>[]
		{
			GetNorthFacingTiles(tiles),
			GetEastFacingTiles(tiles),
			GetSouthFacingTiles(tiles),
			GetWestFacingTiles(tiles),
		};
		int bestIndex = 0;
		int bestCount = -1;
		for (int i = 0; i < groups.Length; ++i)
		{
			int count = 0;
			foreach (Tile t in groups[i])
			{
				if (t.character != null && t.character.hero)
				{
					++count;
				}
			}
			if (count > bestCount)
			{
				bestCount = count;
				bestIndex = i;
			}
		}
		Direction[] directions = new Direction[] { Direction.North, Direction.East, Direction.South, Direction.West };
		return new TilesAndDirection(groups[bestIndex], directions[bestIndex]);
	}

	public TilesAndDirection ThrustTargeting(Character originCharacter)
	{
		List<Tile>[] characterAndDirectionGroupings = new List<Tile>[]
		{
			new List<Tile>(),
			new List<Tile>(),
			new List<Tile>(),
			new List<Tile>()
		};

		for (int i = 0; i < 4; ++i)
		{
			List<Tile> edgeTiles = new List<Tile>();
			Tuple<int, int> directionOffset0 = TileGrid.directions[i];

			foreach (Tile t in GetCharacterEdges(originCharacter, (Direction)i))
			{
				for (int j = 1; j < 4; ++j)
				{
					Tile newT = TileGrid.Instance.GetTile(t.x + directionOffset0.Item1 * j, t.y + directionOffset0.Item2 * j);
					if (newT)
					{
						edgeTiles.Add(newT);
					}
				}
			}
			characterAndDirectionGroupings[i] = edgeTiles;
		}

		return GetLargestDirection(characterAndDirectionGroupings);
	}

	public TilesAndDirection ChopTargeting(Character originCharacter)
	{
		List<Tile>[] characterAndDirectionGroupings = new List<Tile>[]
		{
			new List<Tile>(),
			new List<Tile>(),
			new List<Tile>(),
			new List<Tile>()
		};

		for (int i = 0; i < 4; ++i)
		{
			List<Tile> edgeTiles = new List<Tile>();
			Tuple<int, int> directionOffset0 = TileGrid.directions[i];
			Tuple<int, int> directionOffset1 = TileGrid.directions[(i + 1) % 4];
			Tuple<int, int> directionOffset2 = TileGrid.directions[(i + 3) % 4];

			List<Tile> characterEdgeTiles = GetCharacterEdges(originCharacter, (Direction)i);

			foreach (Tile t in characterEdgeTiles)
			{
				Tile newT = TileGrid.Instance.GetTile(t.x + directionOffset0.Item1, t.y + directionOffset0.Item2);
				if (newT)
				{
					edgeTiles.Add(newT);
				}
			}
			if (i == 3)
			{
				//The west case has the up and down flipped, thus needs to be reversed
				edgeTiles.Reverse();
			}
			if (edgeTiles.Count > 0)
			{
				Tile first = TileGrid.Instance.GetTile(edgeTiles[0].x + directionOffset1.Item1, edgeTiles[0].y + directionOffset1.Item2);
				Tile last = TileGrid.Instance.GetTile(edgeTiles.Last().x + directionOffset2.Item1, edgeTiles.Last().y + directionOffset2.Item2);
				if (first != null)
				{
					edgeTiles.Add(first);
				}
				if (last != null)
				{
					edgeTiles.Add(last);
				}
			}

			characterAndDirectionGroupings[i] = new List<Tile>(edgeTiles);
		}


		return GetLargestDirection(characterAndDirectionGroupings);
	}

	public TilesAndDirection ConeTargeting(Character originCharacter, int range)
	{
		List<Tile>[] characterAndDirectionGroupings = new List<Tile>[]
		{
			new List<Tile>(),
			new List<Tile>(),
			new List<Tile>(),
			new List<Tile>()
		};

		for (int i = 0; i < 4; ++i)
		{
			List<Tile> coneTiles = new List<Tile>();
			List<Tile> characterEdgeTiles = GetCharacterEdges(originCharacter, (Direction)i);

			foreach (Tile t in characterEdgeTiles)
			{
				foreach (Tile coneTile in ActionController.Instance.GetConeTiles((Direction)i, range, t))
				{
					if (!coneTiles.Contains(coneTile))
					{
						coneTiles.Add(coneTile);
					}
				}
			}

			characterAndDirectionGroupings[i] = coneTiles;
		}

		return GetLargestDirection(characterAndDirectionGroupings);
	}

	public TilesAndDirection GetPounceTemplate(Character pouncingCharacter, Character targetCharacter, int range)
	{
		Direction direction = TileGrid.Instance.GetFacingDirection(pouncingCharacter, targetCharacter);
		int intDirection = (int)direction;

		List<Tile> targetedTiles = new List<Tile>();

		List<Tile> tiles = TileGrid.Instance.FindCharacter(pouncingCharacter);
		foreach (Tile t in tiles)
		{
			Tile newTile = TileGrid.Instance.GetTile(t.x + TileGrid.directions[intDirection].Item1, t.y + TileGrid.directions[intDirection].Item2);
			if (newTile == null)
			{
				continue;
			}
			if (tiles.Contains(newTile))
			{
				continue;
			}
			for (int j = 1; j <= range; ++j)
			{
				Tile currentTile = TileGrid.Instance.GetTile(t.x + TileGrid.directions[intDirection].Item1 * j, t.y + TileGrid.directions[intDirection].Item2 * j);
				if (currentTile == null)
				{
					break;
				}
				targetedTiles.Add(currentTile);
			}
		}
		return new TilesAndDirection(targetedTiles, direction);
	}

	public List<Tile> GetTilesInMatchedSizeCardinalDirection(Character originCharacter, int range, int direction)
	{
		List<Tile> outTiles = new List<Tile>();

		List<Tile> tiles = TileGrid.Instance.FindCharacter(originCharacter);
		foreach (Tile t in tiles)
		{
			Tile newTile = TileGrid.Instance.GetTile(t.x + TileGrid.directions[direction].Item1, t.y + TileGrid.directions[direction].Item2);
			if (newTile == null)
			{
				continue;
			}
			if (tiles.Contains(newTile))
			{
				continue;
			}
			for (int j = 1; j <= range; ++j)
			{
				Tile currentTile = TileGrid.Instance.GetTile(t.x + TileGrid.directions[direction].Item1 * j, t.y + TileGrid.directions[direction].Item2 * j);
				if (currentTile == null)
				{
					break;
				}
				outTiles.Add(currentTile);
			}
		}
		return outTiles;
	}

	public TilesAndDirection GetMostCharactersInCardinalDirections(Character originCharacter, int range)
	{
		List<Character>[] characterAndDirectionGroupings = new List<Character>[]
		{
			new List<Character>(),
			new List<Character>(),
			new List<Character>(),
			new List<Character>()
		};
		List<Tile>[] tileAndDirectionGroupings = new List<Tile>[]
		{
			null,
			null,
			null,
			null
		};

		
		for (int i = 0; i < 4; ++i)
		{
			tileAndDirectionGroupings[i] = GetTilesInMatchedSizeCardinalDirection(originCharacter, range, i);
			foreach (Tile t in tileAndDirectionGroupings[i])
			{
				if (t.character && t.character.hero && t.character.GetComponent<Downed>() == null)
				{
					characterAndDirectionGroupings[i].Add(t.character);
				}
			}
		}
		

		int largestIndex = 0;
		for (int i = 0; i < characterAndDirectionGroupings.Length; ++i)
		{
			characterAndDirectionGroupings[i] = characterAndDirectionGroupings[i].Distinct().ToList();
			if (characterAndDirectionGroupings[i].Count > characterAndDirectionGroupings[largestIndex].Count)
			{
				largestIndex = i;
			}
		}

		TilesAndDirection tilesAndDirection = new TilesAndDirection(
													tileAndDirectionGroupings[largestIndex],
													(Direction)largestIndex);

		return tilesAndDirection;
	}

	public List<Tile> GetCloseAoE(Character character)
	{
		List<Tile> tiles = new List<Tile>();
		List<Tile> characterTiles = TileGrid.Instance.FindCharacter(character);

		foreach (Tile t in TileGrid.Instance.tiles)
		{
			foreach (Tile characterTile in characterTiles)
			{
				if (TileGrid.Instance.AreTilesAdjacentOrDiagonal(t, characterTile))
				{
					if (!tiles.Contains(t))
					{
						tiles.Add(t);
					}
				}
			}
		}

		foreach (Tile t in characterTiles)
		{
			tiles.Remove(t);
		}

		bool found = false;
		foreach (Tile t in tiles)
		{
			if (t.character && t.character.hero)
			{
				found = true;
				break;
			}
		}

		if (found)
		{
			tiles.RemoveAll(t => t.character != null && t.character.GetComponent<Stasis>() != null);
			return tiles;
		}
		else
		{
			return null;
		}
	}

	public delegate bool TargetClause(Character c);

	public static List<Tile> GetAdjacentCharacterTarget(Character targetingCharacter, TargetClause targetClause)
	{
		List<Tile> tiles = TileGrid.Instance.GetAllAdjacentTilesToCharacter(targetingCharacter);
		List<Character> targets = new List<Character>();
		foreach (Tile tile in tiles)
		{
			if (tile.character && tile.character.hero)
			{
				if (targetClause != null)
				{
					if (!targetClause(tile.character))
					{
						continue;
					}
				}

				if (!targets.Contains(tile.character))
				{
					targets.Add(tile.character);
				}
			}
		}
		if (targets.Count > 0)
		{
			return TileGrid.Instance.FindCharacter(targets[0]);
		}
		return null;
	}
}
