using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Intimidate : ReactionBase
{
	Tuple<List<Tile>, Tile> route;
	Character currentTarget;
	bool didAggravatedPass = false;

	public override void Execute()
	{
		MoveToTarget(attackingCharacter);
	}

	void MoveToTarget(Character target)
	{
		Dictionary<Character, Tuple<List<Tile>, Tile>> routes = RouteToAllClosestCharacters(true);

		if (target == null || !routes.ContainsKey(target) || routes[target] == null)
		{
			ContinueOrFinish();
			return;
		}

		currentTarget = target;
		route = routes[target];
		Util.ShortenPathToMaxRange(route, owningCharacter.characterDefinition.movement + 1);
		AnimationController.Instance.ShowTiles(route.Item1, Tile.OverlayType.PossibleMovement, ReturnFromShowingTiles, ReturnFromRoute);
	}

	void ReturnFromShowingTiles()
	{
		TileGrid.Instance.RouteAICharacterToTile(owningCharacter, new List<Tile>(route.Item1), ReturnFromRoute, true, true, true);
	}

	void ReturnFromRoute()
	{
		foreach (Tile t in route.Item1)
		{
			t.HideOverlay(Tile.OverlayType.PossibleMovement);
		}

		// Moving provokes reactions/opportunity attacks, so the character may have
		// died on the way to the target. If so, do nothing further and hand control back.
		if (!owningCharacter.alive)
		{
			BattleController.ReturnControlToPlayer();
			return;
		}

		owningCharacter.SetFacing(TileGrid.Instance.GetFacingDirection(owningCharacter, currentTarget));
		if (TileGrid.Instance.CharactersAreAdjacent(owningCharacter, currentTarget))
		{
			currentTarget.AddStatusEffect(typeof(KnockedDown), null);
		}
		else
		{
			FloatingCombatNumberController.Instance.QueueFloatingCombatNumber(owningCharacter, "out of range");
		}

		ContinueOrFinish();
	}

	// After the attacker pass, if the card is aggravated, repeat once more targeting
	// the farthest enemy we can still reach.
	void ContinueOrFinish()
	{
		if (!didAggravatedPass && owningCharacter.alive && GetBoolValue("Aggravated"))
		{
			didAggravatedPass = true;
			Character farthest = GetFarthestReachableTarget();
			if (farthest != null)
			{
				MoveToTarget(farthest);
				return;
			}
		}

		BattleController.ReturnControlToPlayer();
	}

	Character GetFarthestReachableTarget()
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

	public static List<CardInstruction> GetCardInstructions(CardScriptableObject scriptableObject)
	{
		DisplayGrid.Instance.Clear(12, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction("Move to the attacker"));
		instructions.Add(new CardInstruction("Knock them down if adjacent"));
		if (scriptableObject.GetTagBoolValue("Aggravated"))
		{
			instructions.Add(new CardInstruction("Aggravated: repeat on the farthest reachable enemy"));
		}
		return instructions;
	}
}
