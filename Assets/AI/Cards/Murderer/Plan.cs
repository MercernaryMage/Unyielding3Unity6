using System;
using System.Collections.Generic;
using UnityEngine;

public class Plan : Card
{
	Character target;

	bool IsIsolated(Character c)
	{
		if (c.GetComponent<Downed>())
		{
			return false;
		}
		return TileGrid.Instance.IsTargetIsolated(c, owningCharacter);
	}

	Character FindClosestHero(Dictionary<Character, Tuple<List<Tile>, Tile>> routes, bool isolatedOnly)
	{
		Character closest = null;
		int shortest = int.MaxValue;
		foreach (KeyValuePair<Character, Tuple<List<Tile>, Tile>> pair in routes)
		{
			if (!pair.Key.alive)
			{
				continue;
			}
			if (isolatedOnly && !IsIsolated(pair.Key))
			{
				continue;
			}
			if (pair.Value.Item1.Count < shortest)
			{
				shortest = pair.Value.Item1.Count;
				closest = pair.Key;
			}
		}
		return closest;
	}

	public override void Execute()
	{
		Dictionary<Character, Tuple<List<Tile>, Tile>> routes = RouteToAllClosestCharacters(true);
		target = FindClosestHero(routes, true);
		if (target == null)
		{
			target = FindClosestHero(routes, false);
		}
		AnimationController.Instance.ShowTiles(null, Tile.OverlayType.PossibleMovement, Delay, null, 3);
	}

	void Delay()
	{
		if (target != null)
		{
			target.AddStatusEffect(typeof(Marked), null);
		}
		AIController.Instance.Reshuffle(owningCharacter);
		AIController.Instance.TakeTurn(owningCharacter);
	}

	public static List<CardInstruction> GetCardInstructions(CardScriptableObject scriptableObject)
	{
		DisplayGrid.Instance.Clear(11, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction("Apply Marked to the closest isolated enemy"));
		instructions.Add(new CardInstruction("or closest enemy if none are isolated"));
		instructions.Add(new CardInstruction("Do next action"));
		return instructions;
	}
}
