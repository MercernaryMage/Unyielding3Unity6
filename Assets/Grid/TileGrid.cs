using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEditor.VersionControl;
using UnityEngine.TextCore.Text;

public enum Direction
{
	North,
	East,
	South,
	West
}

public class TileGrid : SceneSingleton<TileGrid>
{


	public static List<Tuple<int, int>> directions = new List<Tuple<int, int>>()
	{
		new Tuple<int, int>(0, 1),
		new Tuple<int, int>(1, 0),
		new Tuple<int, int>(0, -1),
		new Tuple<int, int>(-1, 0),
	};

	public List<Tile> tiles;
	public int width;
	public int height;

	public void Init(int w, int h, List<Tile> tileCollection)
	{
		tiles = tileCollection;
		width = w;
		height = h;
	}

	//AI moving
	public void RouteAICharacterToTile(Character character, List<Tile> tiles, Action callback, bool moveCharacter = true, bool provokeReactions = true, bool isReaction = false)
	{
		CharacterStartMovementMessage characterStartMovementMessage = new CharacterStartMovementMessage();
		characterStartMovementMessage.movingCharacter = character;
		characterStartMovementMessage.provokeTriggers = provokeReactions;
		MessagePump.Instance.SendMessage(characterStartMovementMessage);
		Action continueAction = () =>
		{
			PathFollower pathFollower = character.token.gameObject.AddComponent<PathFollower>();
			pathFollower.Set(character, tiles, callback, moveCharacter);
		};
		Action abandonAction = () =>
		{
			foreach (Tile t in TileGrid.Instance.tiles)
			{
				t.HideOverlay(Tile.OverlayType.PossibleMovement);
			}
			if (character.hero)
			{
				HeroDisplayRouter.Instance.mainDisplay.Hide(true);
			}
			else
			{
				if (!isReaction)
				{
					Card.Finish();
				}
			}
		};
		if (characterStartMovementMessage.raisedTriggers.Count == 0)
		{
			continueAction();
		}
		else
		{
			TriggerDisplay.Instance.ShowTriggerMenu(characterStartMovementMessage.raisedTriggers, continueAction, abandonAction);
		}
	}

	public void RouteHeroCharacterToTile(Character character, List<Tile> tiles, Action callback, bool moveCharacter = true, bool provokeReactions = true)
	{
		CharacterStartMovementMessage characterStartMovementMessage = new CharacterStartMovementMessage();
		characterStartMovementMessage.movingCharacter = character;
		characterStartMovementMessage.provokeTriggers = provokeReactions;
		MessagePump.Instance.SendMessage(characterStartMovementMessage);
		Action continueAction = () =>
		{
			PathFollower pathFollower = character.token.gameObject.AddComponent<PathFollower>();
			pathFollower.Set(character, tiles, callback, moveCharacter);
		};
		Action abandonAction = () =>
		{
			foreach (Tile t in TileGrid.Instance.tiles)
			{
				t.HideOverlay(Tile.OverlayType.PossibleMovement);
			}
			if (character.hero)
			{
				HeroDisplayRouter.Instance.mainDisplay.Hide(true);
			}
			else
			{
				Card.Finish();
			}
		};
		if (characterStartMovementMessage.raisedTriggers.Count == 0)
		{
			continueAction();
		}
		else
		{
			TriggerDisplay.Instance.ShowTriggerMenu(characterStartMovementMessage.raisedTriggers, continueAction, abandonAction);
		}
	}

	public void MoveCharacterToTile(Character character, Tile t)
	{
		RemoveCharacter(character);
		PlaceCharacter(t.x, t.y, character);
	}

	public void PlaceCharacter(int x, int y, Character character)
	{
		List<Tuple<int, int>> offsets = GetFittingOffsets(character.characterDefinition.size);
		List<Tile> tiles = new List<Tile>();
		foreach (Tuple<int, int> tuple in offsets)
		{
			int newX = x + tuple.Item1;
			int newY = y + tuple.Item2;
			tiles.Add(GetTile(newX, newY));
		}
		foreach (Tile t in tiles)
		{
			t.EnterTile(character);
		}
		tiles[0].PlaceInTile(character);
	}

	List<Tuple<int, int>> GetFittingOffsets(int size)
	{
		if (size == 1)
		{
			List<int> foo = new List<int>() { 1 };
			return new List<Tuple<int, int>>()
			{
				new Tuple<int, int>(0,0)
			};
		}
		else if (size == 2)
		{
			return new List<Tuple<int, int>>()
			{
				new Tuple<int, int>(0,0),
				new Tuple<int, int>(1,0),
				new Tuple<int, int>(0,1),
				new Tuple<int, int>(1,1)
			};
		}
		else
		{
			List<Tuple<int, int>> offsets = new List<Tuple<int, int>>();
			for (int y = 0; y < size; ++y)
			{
				for (int x = 0; x < size; ++x)
				{
					offsets.Add(new Tuple<int, int>(x, y));
				}
			}
			return offsets;
		}
	}

	public Tile GetTrueTileFromOffset(Tile t, int offsetIndex, int size)
	{
		List<Tuple<int, int>> offsets = GetFittingOffsets(size);
		int newX = t.x - offsets[offsetIndex].Item1;
		int newY = t.y - offsets[offsetIndex].Item2;

		return GetTile(newX, newY);
	}

	public Tile GetTile(int x, int y)
	{
		if (x < 0)
		{
			return null;
		}
		if (x >= width)
		{
			return null;
		}
		if (y < 0)
		{
			return null;
		}
		if (y >= height)
		{
			return null;
		}

		return tiles[y * width + x];
	}

	public List<Tile> FindCharacter(Character character)
	{
		List<Tile> outTiles = new List<Tile>();

		int i = 0;
		for (; i < tiles.Count; ++i)
		{
			if (tiles[i].character == character)
			{
				break;
			}
		}

		int x = i % width;
		int y = i / width;

		List<Tuple<int, int>> offsets = GetFittingOffsets(character.characterDefinition.size);
		foreach (Tuple<int, int> tuple in offsets)
		{
			int newX = x + tuple.Item1;
			int newY = y + tuple.Item2;
			outTiles.Add(tiles[newX + newY * width]);
		}

		return outTiles;
	}

	public List<Tile> GetAdjacentTiles(Tile t)
	{
		List<Tile> outTiles = new List<Tile>();

		if (t.x - 1 >= 0)
		{
			outTiles.Add(GetTile(t.x - 1, t.y));
		}
		if (t.x + 1 < width)
		{
			outTiles.Add(GetTile(t.x + 1, t.y));
		}

		if (t.y - 1 >= 0)
		{
			outTiles.Add(GetTile(t.x, t.y - 1));
		}
		if (t.y + 1 < height)
		{
			outTiles.Add(GetTile(t.x, t.y + 1));
		}

		return outTiles;
	}

	public bool AreTilesAdjacent(Tile t0, Tile t1)
	{
		List<Tuple<int, int>> directions = new List<Tuple<int, int>>()
		{
			new Tuple<int, int>(0, -1),
			new Tuple<int, int>(-1, 0),
			new Tuple<int, int>(1, 0),
			new Tuple<int, int>(0, 1),
		};

		for (int i = 0; i < directions.Count; ++i)
		{
			if (t0.x + directions[i].Item1 == t1.x &&
				t0.y + directions[i].Item2 == t1.y)
			{
				return true;
			}
		}

		return false;
	}

	public bool AreTilesAdjacentOrDiagonal(Tile t0, Tile t1)
	{
		List<Tuple<int, int>> directions = new List<Tuple<int, int>>()
		{
			new Tuple<int, int>(-1, -1),
			new Tuple<int, int>(0, -1),
			new Tuple<int, int>(1, -1),
			new Tuple<int, int>(-1, 0),
			new Tuple<int, int>(1, 0),
			new Tuple<int, int>(-1, 1),
			new Tuple<int, int>(0, 1),
			new Tuple<int, int>(1, 1),
		};

		for (int i = 0; i < directions.Count; ++i)
		{
			if (t0.x + directions[i].Item1 == t1.x &&
				t0.y + directions[i].Item2 == t1.y)
			{
				return true;
			}
		}

		return false;
	}

	public void RemoveCharacter(Character c)
	{
		List<Tile> tiles = FindCharacter(c);
		foreach (Tile t in tiles)
		{
			t.character = null;
		}
	}

	public bool WouldCharacterFitAtTile(Character c, Tile tile)
	{
		List<Tile> tiles = WhatTilesWouldCharacterTake(c, tile);
		if (tiles == null)
		{
			return false;
		}
		foreach (Tile t in tiles)
		{
			if (!t.tileScriptableObject.enterable)
			{
				return false;
			}
			if (t.character && t.character != c)
			{
				return false;
			}
		}
		return true;
	}

	public List<Tile> WhatTilesWouldCharacterTake(Character c, Tile startTile)
	{
		int x = startTile.x;
		int y = startTile.y;
		List<Tuple<int, int>> offsets = GetFittingOffsets(c.characterDefinition.size);
		List<Tile> outTiles = new List<Tile>();
		foreach (Tuple<int, int> tuple in offsets)
		{
			int newX = x + tuple.Item1;
			int newY = y + tuple.Item2;
			Tile newT = GetTile(newX, newY);
			if (newT == null)
			{
				return null;
			}
			if (!newT.tileScriptableObject.enterable)
			{
				return null;
			}
			outTiles.Add(newT);
		}
		return outTiles;
	}

	public List<Tile> GetAllAdjacentTilesToCharacter(Character c)
	{
		List<Tile> fitTiles = FindCharacter(c);
		List<Tile> allTiles = new List<Tile>();
		foreach (Tile t in fitTiles)
		{
			allTiles.AddRange(GetAdjacentTiles(t));
		}
		return allTiles.Distinct().ToList();
	}

	public List<Tile> GetSideTiles(Character c)
	{
		List<Tile> tiles = new List<Tile>();

		List<Tile> characterTiles = FindCharacter(c);
		foreach (Tile t in characterTiles)
		{
			if (c.facing == Direction.North ||
				c.facing == Direction.South)
			{
				tiles.Add(GetTile(t.x + 1, t.y));
				tiles.Add(GetTile(t.x - 1, t.y));
			}
			else
			{
				tiles.Add(GetTile(t.x, t.y + 1));
				tiles.Add(GetTile(t.x, t.y - 1));
			}
		}

		for (int i = 0; i < tiles.Count; ++i)
		{
			if (tiles[i] == null || tiles[i].character == c)
			{
				tiles.RemoveAt(i);
				--i;
				continue;
			}
		}

		return tiles;
	}

	public static int Distance(Tile t0, Tile t1)
	{
		return Mathf.Abs(t0.x - t1.x) + Mathf.Abs(t0.y - t1.y);
	}

	public Tile GetClosestCharacterTile(Tile tile, Character character)
	{
		List<Tile> characterTiles = FindCharacter(character);
		Tile closestTile = null;
		int closestDistance = int.MaxValue;

		foreach (Tile t in characterTiles)
		{
			int distance = Distance(tile, t);
			if (distance < closestDistance)
			{
				closestDistance = distance;
				closestTile = t;
			}
		}

		return closestTile;
	}

	//Hey.  This is a VERY simple implementation of the complicated
	//solution.
	public Character FindClosestEnemy(Character c)
	{
		Tile t = FindCharacter(c)[0];
		Tile closestTile = null;
		foreach (Character enemy in BattleController.Instance.enemies)
		{
			if (!enemy.alive)
			{
				continue;
			}
			List<Tile> enemyTiles = FindCharacter(enemy);
			foreach (Tile enemyTile in enemyTiles)
			{
				if (closestTile == null || TileGrid.Distance(t, closestTile) > TileGrid.Distance(t, enemyTile))
				{
					closestTile = enemyTile;
				}
			}

		}

		return closestTile.character;
	}

	public Character FindClosestHero(Tile fromTile)
	{
		Character closestHero = null;
		float closestDistance = float.MaxValue;
		foreach (Character hero in BattleController.Instance.heroes)
		{
			if (!hero.alive) continue;
			List<Tile> heroTiles = FindCharacter(hero);
			if (heroTiles != null && heroTiles.Count > 0)
			{
				float distance = Vector3.Distance(fromTile.transform.position, heroTiles[0].transform.position);
				if (distance < closestDistance)
				{
					closestDistance = distance;
					closestHero = hero;
				}
			}
		}
		return closestHero;
	}
	
	public Direction GetFacingDirection(Character c0, Character c1)
	{
		Tile t0 = FindCharacter(c0)[0];
		Tile t1 = GetClosestCharacterTile(t0, c1);
		t0 = GetClosestCharacterTile(t1, c0);
		return GetFacingDirection(t0, t1);
	}

	public Direction GetFacingDirection(Tile t0, Tile t1)
	{
		if (Mathf.Abs(t0.x - t1.x) > Mathf.Abs(t0.y - t1.y))
		{
			if (t0.x - t1.x > 0)
			{
				return Direction.West;
			}
			else
			{
				return Direction.East;
			}
		}
		else
		{
			if (t0.y - t1.y > 0)
			{
				return Direction.South;
			}
			else
			{
				return Direction.North;
			}
		}

	}

	public bool CharactersAreAdjacent(Character c1, Character c2)
	{
		List<Tile> tiles = GetAllAdjacentTilesToCharacter(c1);
		foreach (Tile t in tiles)
		{
			if (t.character == c2)
			{
				return true;
			}
		}
		return false;
	}

	public static bool TilesContainHero(List<Tile> tiles)
	{
		foreach (Tile t in tiles)
		{
			if (t.character && t.character.hero)
			{
				return true;
			}
		}

		return false;
	}

	public static bool AreTilesCardinal(Tile t0, Tile t1)
	{
		return t0.x == t1.x || t0.y == t1.y;
	}

	public static Vector2 FindCenter(List<Tile> tiles)
	{
		float sumX = 0;
		float sumY = 0;
		foreach (Tile t in tiles)
		{
			sumX += t.x;
			sumY += t.y;
		}
		return new Vector2(sumX / tiles.Count + .5f, sumY / tiles.Count + .5f);
	}

	public static bool AreCharactersAdjacent(Character c1, Character c2)
	{
		return AreCharactersAdjacentInner(c1, c2) | AreCharactersAdjacentInner(c2, c1);
	}

	public static bool AreCharactersAdjacentInner(Character c1, Character c2)
	{
		List<Tile> t1s = Instance.GetAllAdjacentTilesToCharacter(c1);
		List<Tile> t2s = Instance.FindCharacter(c2);

		return t1s.Intersect(t2s).ToList().Count > 0;

	}

	static public Direction GetOppositeFacing(Direction direction)
	{
		if (direction == Direction.North)
		{
			return Direction.South;
		}
		else if (direction == Direction.South)
		{
			return Direction.North;
		}
		else if (direction == Direction.West)
		{
			return Direction.East;
		}
		else
		{
			return Direction.West;
		}
	}

	public bool IsFacing(Character c0, Character c1)
	{
		List<Tile> tiles1 = FindCharacter(c0);
		List<Tile> tiles2 = FindCharacter(c1);

		foreach (Tile t1 in tiles1)
		{
			foreach (Tile t2 in tiles2)
			{
				if (c0.facing == Direction.South)
				{
					if (t1.y < t2.y)
					{
						return false;
					}
				}
				else if (c0.facing == Direction.North)
				{
					if (t1.y > t2.y)
					{
						return false;
					}
				}
				else if (c0.facing == Direction.West)
				{
					if (t1.x < t2.x)
					{
						return false;
					}
				}
				else
				{
					if (t1.x > t2.x)
					{
						return false;
					}
				}
			}
		}
		return true;
	}

	public List<Tile> GetAllTilesInRange(Tile origin, int range)
	{
		List<Tile> outTiles = new List<Tile>();
		foreach (Tile t in tiles)
		{
			if (Distance(origin, t) <= range)
			{
				outTiles.Add(t);
			}
		}
		return outTiles;
	}

	public void HideAllTiles()
	{
		foreach (Tile t in tiles)
		{
			t.HideAllOverlays();
		}
	}

	public int GetDistanceBetweenCharacters(Character c1, Character c2)
	{
		Tile firstTile = TileGrid.Instance.FindCharacter(c1)[0];
		Tile secondTile = TileGrid.Instance.GetClosestCharacterTile(firstTile, c2);
		firstTile = TileGrid.Instance.GetClosestCharacterTile(secondTile, c1);
		int distance = TileGrid.Distance(firstTile, secondTile);
		return distance;
	}

	public int GetDistanceBetweenCharacterAndTile(Character c, Tile t)
	{
		Tile characterTile = TileGrid.Instance.GetClosestCharacterTile(t, c);
		return Distance(characterTile, t);
	}

	public bool IsDestinationAdjacentToCharacter(Character owningCharacter, Tile destinationTile, bool hero)
	{
		List<Tile> wouldOccupy = TileGrid.Instance.WhatTilesWouldCharacterTake(owningCharacter, destinationTile);
		if (wouldOccupy == null)
		{
			return false;
		}

		List<Character> characters = hero ? BattleController.Instance.heroes : BattleController.Instance.enemies;
		foreach (Character c in characters)
		{
			if (!c.alive)
			{
				continue;
			}
			if (c.gameObject.GetComponent<Downed>() != null)
			{
				continue;
			}

			List<Tile> heroTiles = TileGrid.Instance.FindCharacter(c);
			foreach (Tile myTile in wouldOccupy)
			{
				foreach (Tile heroTile in heroTiles)
				{
					if (TileGrid.Instance.AreTilesAdjacent(myTile, heroTile))
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	public bool IsTargetIsolated(Character target, Character user)
	{
		List<Tile> adjacentTiles = GetAllAdjacentTilesToCharacter(target);
		foreach (Tile tile in adjacentTiles)
		{
			if (tile.character != null &&
				tile.character != target &&
				tile.character != user)
			{
				return false;
			}
		}

		return true;
	}

	public bool DoesTileHaveLOSToCharacter(Tile t0, Character c0)
	{
		List<Tile> characerTiles = FindCharacter(c0);
		foreach (Tile tile in characerTiles)
		{
			if (!DoesTileHaveLOSToTile(t0, tile))
			{
				return false;
			}
		}
		return true;
	}

	public bool DoesCharacterHaveLOSToCharacter(Character c0, Character c1)
	{
		List<Tile> c0Tiles = FindCharacter(c0);
		List<Tile> c1Tiles = FindCharacter(c1);
		foreach (Tile t0 in c0Tiles)
		{
			foreach (Tile t1 in c1Tiles)
			{
				if (DoesTileHaveLOSToTile(t0, t1))
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool DoesTileHaveLOSToTile(Tile t0, Tile t1)
	{
		List<Tile> collisions = GetLineCollisions(t0, t1);

		foreach (Tile collision in collisions)
		{
			if (collision.tileScriptableObject.blocksLOS)
			{
				return false;
			}
		}
		return true;
	}

	public List<Tile> GetLineTilesTillCollision(Tile t0, Tile t1)
	{
		List<Tile> collisions = GetLineCollisions(t0, t1);

		collisions.Sort((a, b) =>
			Vector3.Distance(t0.transform.position, a.transform.position)
			.CompareTo(Vector3.Distance(t0.transform.position, b.transform.position)));

		List<Tile> outTiles = new List<Tile>();

		foreach (Tile collision in collisions)
		{
			if (collision.tileScriptableObject.blocksLOS)
			{
				break;
			}
			outTiles.Add(collision);
		}

		return outTiles;
	}

	public List<Tile> GetLineCollisions(Tile t0, Tile t1)
	{
		List<Tile> collisions = new List<Tile>();

		RaycastHit[] hits = Physics.RaycastAll(t0.transform.position, (t1.transform.position - t0.transform.position).normalized, (t1.transform.position - t0.transform.position).magnitude);
		foreach (RaycastHit hit in hits)
		{
			Tile hitTile = hit.collider.gameObject.transform.GetComponent<Tile>();
			if (hitTile != null)
			{
				collisions.Add(hitTile);
			}
		}

		return collisions;
	}
}
