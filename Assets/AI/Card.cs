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
						Tuple<List<Tile>, Tile> data = new Tuple<List<Tile>, Tile>(MovementController.Instance.FindRoute(owningCharacter, trueTile, i, rules), trueTile);
						if (data.Item1 != null)
						{
							routes[target] = data;
						}
					}
					else
					{
						List<Tile> newRoute = MovementController.Instance.FindRoute(owningCharacter, trueTile, i, rules);
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

					List<Tile> path = MovementController.Instance.FindRoute(owningCharacter, trueTile, i, rules);
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
