using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum InstructionType
{
	String,
	Grid,
}

public class CardInstruction
{
	public CardInstruction()
	{
		instruction = InstructionType.Grid;
	}
	public CardInstruction(string s)
	{
		instruction = InstructionType.String;
		instructionWords = s;
	}
	public InstructionType instruction;
	public string instructionWords;
}

[Serializable]
public class Card 
{
	public Character owningCharacter;
	public CardScriptableObject cardScriptableObject;

	public bool isRevealed = false;

	public void Set(CardScriptableObject c)
	{
		cardScriptableObject = c;
	}
	
	virtual public void Execute()
	{

	}

	static public void Finish()
	{
		if (ActionController.Instance.queuedActions.Count > 0)
		{
			ActionController.Instance.DoQueuedAction();
			return;
		}
		BattleController.Instance.CardFinished();
	}

	public Dictionary<Character, Tuple<List<Tile>, Tile>> RouteToAllClosestCharacters(bool targetHeroes)
	{
		Dictionary<Character, Tuple<List<Tile>, Tile>> routes = new Dictionary<Character, Tuple<List<Tile>, Tile>>();

		List<Character> targets = targetHeroes ? BattleController.Instance.heroes : BattleController.Instance.enemies;

		foreach (Character target in targets)
		{
			if (target == owningCharacter)
			{
				continue;
			}

			if (!target.alive)
			{
				continue;
			}

			if (target.gameObject.GetComponent<Downed>() != null)
			{
				continue;
			}

			List<Tile> adjacentTiles = TileGrid.Instance.GetAllAdjacentTilesToCharacter(target);

			int tileSize = owningCharacter.characterDefinition.size * owningCharacter.characterDefinition.size;
			for (int i = 0; i < tileSize; ++i)
			{
				foreach (Tile t in adjacentTiles)
				{
					Tile trueTile = TileGrid.Instance.GetTrueTileFromOffset(t, i, owningCharacter.characterDefinition.size);
					if (trueTile == null)
					{
						continue;
					}
					if (!TileGrid.Instance.WouldCharacterFitAtTile(owningCharacter, trueTile))
					{
						continue;
					}

					MovementController.PathfindingRules rules = new MovementController.PathfindingRules();
					rules.allowedToPathThroughAllies = true;

					if (!routes.ContainsKey(target))
					{
						Tuple<List<Tile>, Tile> data = new Tuple<List<Tile>, Tile>(MovementController.Instance.FindRoute(owningCharacter, trueTile, 0, rules), trueTile);
						if (data.Item1 != null)
						{
							routes[target] = data;
						}
					}
					else
					{
						List<Tile> newRoute = MovementController.Instance.FindRoute(owningCharacter, trueTile, 0, rules);
						if (newRoute != null && routes[target].Item1.Count > newRoute.Count)
						{
							routes[target] = new Tuple<List<Tile>, Tile>(newRoute, trueTile);
						}
					}
				}
			}
		}

		return routes;
	}

	public Character GetFarthestReachableTarget(Character attackingCharacter)
	{
		Dictionary<Character, Tuple<List<Tile>, Tile>> routes = RouteToAllClosestCharacters(true);
		int maxRange = owningCharacter.characterDefinition.movement + 1;

		Character farthest = null;
		int farthestLength = -1;
		foreach (KeyValuePair<Character, Tuple<List<Tile>, Tile>> pair in routes)
		{
			if (pair.Key == attackingCharacter || pair.Value == null || pair.Value.Item1 == null)
			{
				continue;
			}

			int length = pair.Value.Item1.Count;
			if (length > maxRange)
			{
				continue;
			}

			if (length > farthestLength)
			{
				farthestLength = length;
				farthest = pair.Key;
			}
		}

		return farthest;
	}

	public Tuple<List<Tile>, Tile> FindRouteAdjacentToBoth()
	{
		Tuple<List<Tile>, Tile> bestRoute = null;

		foreach (Character ally in BattleController.Instance.enemies)
		{
			if (ally == owningCharacter)
			{
				continue;
			}
			if (!ally.alive)
			{
				continue;
			}
			if (ally.gameObject.GetComponent<Downed>() != null)
			{
				continue;
			}

			List<Tile> adjacentToAlly = TileGrid.Instance.GetAllAdjacentTilesToCharacter(ally);
			int tileSize = owningCharacter.characterDefinition.size * owningCharacter.characterDefinition.size;

			for (int i = 0; i < tileSize; ++i)
			{
				foreach (Tile t in adjacentToAlly)
				{
					Tile trueTile = TileGrid.Instance.GetTrueTileFromOffset(t, i, owningCharacter.characterDefinition.size);
					if (trueTile == null)
					{
						continue;
					}
					if (!TileGrid.Instance.WouldCharacterFitAtTile(owningCharacter, trueTile))
					{
						continue;
					}
					if (!TileGrid.Instance.IsDestinationAdjacentToCharacter(owningCharacter, trueTile, true))
					{
						continue;
					}

					MovementController.PathfindingRules rules = new MovementController.PathfindingRules();
					rules.allowedToPathThroughAllies = true;

					List<Tile> path = MovementController.Instance.FindRoute(owningCharacter, trueTile, 0, rules);
					if (path == null)
					{
						continue;
					}

					if (bestRoute == null || path.Count < bestRoute.Item1.Count)
					{
						bestRoute = new Tuple<List<Tile>, Tile>(path, trueTile);
					}
				}
			}
		}

		return bestRoute;
	}

	//if any enemy is within threatRange, moves to a random one of the reachable tiles
	//farthest from all enemies, then calls onComplete; calls onComplete immediately if
	//no move is needed or no tile is farther than the current one
	public void MoveAwayIfNeeded(Action onComplete, int threatRange = 3)
	{
		bool foundAny = false;
		foreach (Character hero in BattleController.Instance.heroes)
		{
			if (!hero.alive || hero.GetComponent<Downed>() != null)
			{
				continue;
			}
			if (TileGrid.Instance.GetDistanceBetweenCharacters(hero, owningCharacter) <= threatRange)
			{
				foundAny = true;
				break;
			}
		}

		if (!foundAny)
		{
			onComplete();
			return;
		}

		List<Character> threats = new List<Character>();
		foreach (Character hero in BattleController.Instance.heroes)
		{
			if (hero.alive)
			{
				threats.Add(hero);
			}
		}

		int moveRange = owningCharacter.characterDefinition.movement;
		MovementController.PathfindingRules rules = new MovementController.PathfindingRules();
		rules.allowedToPathThroughAllies = true;

		List<List<Tile>> bestRoutes = new List<List<Tile>>();
		int bestMinDist = GetMinDistanceToThreats(TileGrid.Instance.FindCharacter(owningCharacter)[0], threats);

		foreach (Tile tile in TileGrid.Instance.tiles)
		{
			if (tile.character != null)
			{
				continue;
			}
			if (!TileGrid.Instance.WouldCharacterFitAtTile(owningCharacter, tile))
			{
				continue;
			}
			List<Tile> route = MovementController.Instance.FindRoute(owningCharacter, tile, 0, rules);
			if (route == null || route.Count > moveRange + 1)
			{
				continue;
			}

			int minDist = GetMinDistanceToThreats(tile, threats);
			if (minDist > bestMinDist)
			{
				bestMinDist = minDist;
				bestRoutes.Clear();
				bestRoutes.Add(route);
			}
			else if (minDist == bestMinDist && bestRoutes.Count > 0)
			{
				bestRoutes.Add(route);
			}
		}

		if (bestRoutes.Count == 0)
		{
			onComplete();
			return;
		}

		List<Tile> movePath = bestRoutes[UnityEngine.Random.Range(0, bestRoutes.Count)];
		if (movePath.Count <= 1)
		{
			onComplete();
			return;
		}

		List<Tile> litRouteTiles = Util.ExpandPathTiles(movePath, owningCharacter);
		Action hideAndComplete = () =>
		{
			foreach (Tile t in litRouteTiles)
			{
				t.HideOverlay(Tile.OverlayType.PossibleMovement);
			}
			onComplete();
		};
		AnimationController.Instance.ShowTiles(litRouteTiles, Tile.OverlayType.PossibleMovement, () =>
		{
			TileGrid.Instance.RouteAICharacterToTile(owningCharacter, new List<Tile>(movePath), hideAndComplete);
		}, hideAndComplete);
	}

	int GetMinDistanceToThreats(Tile tile, List<Character> threats)
	{
		int minDist = int.MaxValue;
		foreach (Character threat in threats)
		{
			foreach (Tile threatTile in TileGrid.Instance.FindCharacter(threat))
			{
				int dist = TileGrid.Distance(tile, threatTile);
				if (dist < minDist)
				{
					minDist = dist;
				}
			}
		}
		return minDist;
	}

	public string GetCardName()
	{
		return cardScriptableObject.cardDisplayName;
	}

	public int GetIntValue(string s)
	{
		return cardScriptableObject.GetTagIntValue(s);
	}

	public string GetStringValue(string s)
	{
		return cardScriptableObject.GetTagStringValue(s);
	}

	public bool GetBoolValue(string s)
	{
		return cardScriptableObject.GetTagBoolValue(s);
	}
}
