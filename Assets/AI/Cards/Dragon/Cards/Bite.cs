using System;
using System.Collections.Generic;
using UnityEngine;

public class Bite : Card
{
	Tuple<List<Tile>, Tile> route;
	Character target;

	public override void Execute()
	{
		Dictionary<Character, Tuple<List<Tile>, Tile>> routes = RouteToAllClosestCharacters(true);

		route = Util.FindSmallestRoute(routes, null);

		if (route == null)
		{
			Finish();
			return;
		}

		Util.ShortenPathToMaxRange(route, owningCharacter.characterDefinition.movement + 1);
		AnimationController.Instance.ShowTiles(route.Item1, Tile.OverlayType.PossibleMovement, ReturnFromShowingTiles, ReturnFromRoute);
	}

	void ReturnFromShowingTiles()
	{
		TileGrid.Instance.RouteAICharacterToTile(owningCharacter, new List<Tile>(route.Item1), ReturnFromRoute);
	}

	void ReturnFromRoute()
	{
		foreach (Tile t in route.Item1)
		{
			t.HideOverlay(Tile.OverlayType.PossibleMovement);
		}

		if (!TileGrid.AreCharactersAdjacent(target, owningCharacter))
		{
			Finish();
			return;
		}

		List<Tile> targetTiles = TileGrid.Instance.FindCharacter(target);
		AnimationController.Instance.ShowTiles(targetTiles, Tile.OverlayType.PossibleAttck, ReturnFromShowingAttackTiles);
	}

	void ReturnFromShowingAttackTiles()
	{
		owningCharacter.SetFacing(TileGrid.Instance.GetFacingDirection(owningCharacter, target));
		ActionController.Instance.PlayAttackAnimation(owningCharacter, null, () =>
		{
			List<Tile> targetTiles = TileGrid.Instance.FindCharacter(target);
			foreach (Tile t in targetTiles)
			{
				t.HideOverlay(Tile.OverlayType.PossibleAttck);
			}
			if (target.alive)
			{
				ActionController.AttackProfile profile = new ActionController.AttackProfile(1, 6, 0);
				profile.accuracy = -1;
				profile.autoCrit = true;
				ActionController.Instance.AttackCharacter(target, owningCharacter, profile);
			}
			Finish();
		});
	}

	public static List<CardInstruction> GetCardInstructions(CardScriptableObject scriptableObject)
	{
		DisplayGrid.Instance.Clear(11, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction("Move toward closest enemy"));
		instructions.Add(new CardInstruction("Attack for 1d6 damage"));
		instructions.Add(new CardInstruction("-1 accuracy, always crits on hit"));
		DisplayGrid.Instance.Show();
		return instructions;
	}
}
