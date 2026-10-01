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

		Dictionary<Character, Tuple<List<Tile>, Tile>> routes = RouteToAllClosestCharacters(true);
		if (!routes.ContainsKey(target))
		{
			AnimationController.Instance.DelayedCallback(.5f, ReturnFromRoute);
			return;
		}

		route = routes[target];
		Util.ShortenPathToMaxRange(route, owningCharacter.characterDefinition.movement + 1);

		litRouteTiles = Util.ExpandPathTiles(route.Item1, owningCharacter);
		AnimationController.Instance.ShowTiles(litRouteTiles, Tile.OverlayType.PossibleMovement, Route, ReturnFromRoute);
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
					ActionController.Instance.AttackCharacter(t.character, owningCharacter, new ActionController.AttackProfile(1, 6, 0));
				}
				t.HideOverlay(Tile.OverlayType.PossibleAttck);
			}

			FinishChase();
		});
	}

	void FinishChase()
	{
		Action callback = chaseCompleteCallback;
		chaseCompleteCallback = null;
		if (callback != null)
		{
			callback();
		}
	}

	public static List<CardInstruction> GetCardInstructions(CardScriptableObject scriptableObject)
	{
		DisplayGrid.Instance.Clear(11, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction("Mark the tile of the closest isolated enemy"));
		instructions.Add(new CardInstruction("or closest enemy if none are isolated"));
		instructions.Add(new CardInstruction("The mark follows them as they move"));
		instructions.Add(new CardInstruction("At the start of the next turn, move to the marked enemy"));
		instructions.Add(new CardInstruction("and attack it for 1d6 damage"));
		DisplayGrid.Instance.Show();

		return instructions;
	}
}
