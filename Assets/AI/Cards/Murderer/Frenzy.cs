using System;
using System.Collections.Generic;
using UnityEngine;

public class Frenzy : Card
{
	Character target;
	Tuple<List<Tile>, Tile> route;
	List<Tile> litRouteTiles;
	List<Tile> attackTiles;
	List<Character> attackedCharacters;

	bool NotYetAttacked(Character c)
	{
		return !attackedCharacters.Contains(c);
	}

	public override void Execute()
	{
		attackedCharacters = new List<Character>();
		litRouteTiles = new List<Tile>();
		MoveToNextTarget(owningCharacter.characterDefinition.movement + 1);
	}

	void MoveToNextTarget(int maxRange)
	{
		Dictionary<Character, Tuple<List<Tile>, Tile>> routes = RouteToAllClosestCharacters(true);
		KeyValuePair<Character, Tuple<List<Tile>, Tile>> closest = Util.FindSmallestRoutePair(routes, NotYetAttacked);

		target = closest.Key;
		route = closest.Value;
		if (route == null)
		{
			Finish();
			return;
		}

		Util.ShortenPathToMaxRange(route, maxRange);

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
			Finish();
			return;
		}

		attackTiles = TileGrid.Instance.FindCharacter(target);
		if (attackTiles == null || attackTiles.Count == 0)
		{
			Finish();
			return;
		}

		AnimationController.Instance.ShowTiles(attackTiles, Tile.OverlayType.PossibleAttck, ReturnFromShowingAttackTiles);
	}

	public void ReturnFromShowingAttackTiles()
	{
		owningCharacter.SetFacing(TileGrid.Instance.GetFacingDirection(owningCharacter, target));
		ActionController.Instance.PlayAttackAnimation(owningCharacter, null, () =>
		{
			attackedCharacters.Add(target);
			ActionController.AttackResults results = ActionController.Instance.AttackCharacter(target, owningCharacter, new ActionController.AttackProfile(1, 6, 0));

			foreach (Tile t in attackTiles)
			{
				t.HideOverlay(Tile.OverlayType.PossibleAttck);
			}

			if (!results.hit)
			{
				Finish();
				return;
			}

			MoveToNextTarget(2);
		});
	}

	public static List<CardInstruction> GetCardInstructions(CardScriptableObject scriptableObject)
	{
		DisplayGrid.Instance.Clear(11, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction("Move to the closest enemy and attack for 1d6 damage"));
		instructions.Add(new CardInstruction("On a hit, step 1 tile toward the closest enemy"));
		instructions.Add(new CardInstruction("that has not been attacked and attack it"));
		instructions.Add(new CardInstruction("Repeat until a miss, an enemy out of reach,"));
		instructions.Add(new CardInstruction("or every enemy has been attacked"));
		DisplayGrid.Instance.Show();

		return instructions;
	}
}
