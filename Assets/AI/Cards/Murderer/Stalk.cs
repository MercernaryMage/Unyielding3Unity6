using System;
using System.Collections.Generic;
using UnityEngine;

public class Stalk : Card
{
	Character target;
	Tuple<List<Tile>, Tile> route;
	List<Tile> litRouteTiles;
	List<Tile> attackTiles;
	Action chaseCompleteCallback;

	bool IsIsolated(Character c)
	{
		if (c.GetComponent<Recovering>())
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

		if (target == null)
		{
			Debug.Log("No target");
			Finish();
			return;
		}

		Stalked stalked = (Stalked)target.AddStatusEffectAllowDuplicate(typeof(Stalked));
		stalked.Track(0);

		Stalking stalking = (Stalking)owningCharacter.AddStatusEffectAllowDuplicate(typeof(Stalking));
		stalking.Set(this, target, stalked);

		AnimationController.Instance.ScrollToCharacter(target, () => Finish(), .5f);
	}

	public void ChaseTarget(Character chaseTarget, Action onComplete)
	{
		target = chaseTarget;
		chaseCompleteCallback = onComplete;
		litRouteTiles = new List<Tile>();

		AICardDisplay.Instance.Dismiss(true);
		AICardDisplay.Instance.ShowCard(cardScriptableObject, CardDisplay.CardType.Card, owningCharacter);
		CombatLogControl.Instance.AddCard($"{owningCharacter} pursues {target}", this);

		route = FindChaseRoute(target);
		if (route == null)
		{
			AnimationController.Instance.DelayedCallback(.5f, ReturnFromRoute);
			return;
		}

		litRouteTiles = Util.ExpandPathTiles(route.Item1, owningCharacter);
		AnimationController.Instance.ShowTiles(litRouteTiles, Tile.OverlayType.PossibleMovement, Route, ReturnFromRoute);
	}

	Tuple<List<Tile>, Tile> FindChaseRoute(Character chaseTarget)
	{
		int maxRange = owningCharacter.characterDefinition.movement + 1;
		int tileSize = owningCharacter.characterDefinition.size * owningCharacter.characterDefinition.size;

		MovementController.PathfindingRules rules = new MovementController.PathfindingRules();
		rules.allowedToPathThroughAllies = true;

		List<Tile> adjacentTiles = TileGrid.Instance.GetAllAdjacentTilesToCharacter(chaseTarget);

		Tuple<List<Tile>, Tile> bestRoute = null;
		int bestDistance = int.MaxValue;
		int bestSteps = int.MaxValue;

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

				List<Tile> path = MovementController.Instance.FindRoute(owningCharacter, trueTile, 0, rules);
				if (path == null)
				{
					continue;
				}

				int stoppableIndex = Util.FindStoppableIndex(path, owningCharacter, maxRange);
				int distance = TileGrid.Instance.GetDistanceBetweenCharacterAndTile(chaseTarget, path[stoppableIndex]);

				if (distance > bestDistance || (distance == bestDistance && stoppableIndex >= bestSteps))
				{
					continue;
				}

				Tuple<List<Tile>, Tile> candidate = new Tuple<List<Tile>, Tile>(path, trueTile);
				Util.ShortenPathToMaxRange(candidate, stoppableIndex + 1);

				bestDistance = distance;
				bestSteps = stoppableIndex;
				bestRoute = candidate;
			}
		}

		return bestRoute;
	}

	public void Route()
	{
		TileGrid.Instance.RouteAICharacterToTile(owningCharacter, new List<Tile>(route.Item1), ReturnFromRoute);
	}

	public void ReturnFromRoute()
	{
		foreach (Tile t in litRouteTiles)
		{
			t.HideOverlay(Tile.OverlayType.PossibleMovement);
		}

		if (target == null || !target.alive || !TileGrid.AreCharactersAdjacent(target, owningCharacter))
		{
			FloatingCombatNumberController.Instance.QueueFloatingCombatNumber(owningCharacter, "target out of range");
			FinishChase();
			return;
		}

		attackTiles = TileGrid.Instance.FindCharacter(target);
		if (attackTiles == null || attackTiles.Count == 0)
		{
			FinishChase();
			return;
		}

		AnimationController.Instance.ShowTiles(attackTiles, Tile.OverlayType.PossibleAttck, ReturnFromShowingAttackTiles);
	}

	public void ReturnFromShowingAttackTiles()
	{
		owningCharacter.SetFacing(TileGrid.Instance.GetFacingDirection(owningCharacter, target));
		ActionController.Instance.PlayAttackAnimation(owningCharacter, null, () =>
		{
			foreach (Tile t in attackTiles)
			{
				if (t.character && t.character.hero)
				{
					ActionController.Instance.AttackCharacter(t.character, owningCharacter, new ActionController.AttackProfile(1, 6, 2));
				}
				t.HideOverlay(Tile.OverlayType.PossibleAttck);
			}

			FinishChase();
		});
	}

	void FinishChase()
	{
		AICardDisplay.Instance.SoftDismiss(.5f);

		Action callback = chaseCompleteCallback;
		chaseCompleteCallback = null;
		if (callback != null)
		{
			callback();
		}
	}

	public static List<CardInstruction> GetCardInstructions(CardScriptableObject scriptableObject, Character character )
	{
		DisplayGrid.Instance.Clear(11, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction("Mark the tile of the closest isolated enemy or closest enemy if none are isolated and use <i>Charge</i>"));
		instructions.Add(new CardInstruction("The mark follows them as they move"));
		instructions.Add(new CardInstruction("On <u>Charge</u> completion: move to the marked enemy and attack it for 1d6+2 damage"));
		DisplayGrid.Instance.Show();

		return instructions;
	}
}
