using System;
using System.Collections.Generic;
using UnityEngine;
using static ActionController;

public class FightingDirty : Card
{
	Tuple<List<Tile>, Tile> route;
	List<Tile> litRouteTiles;
	List<Tile> targetTiles;
	bool moved;

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
		moved = route.Item1.Count > 1;

		if (!moved)
		{
			Attack();
			return;
		}

		litRouteTiles = Util.ExpandPathTiles(route.Item1, owningCharacter);
		AnimationController.Instance.ShowTiles(litRouteTiles, Tile.OverlayType.PossibleMovement, ReturnFromShowingMovementTiles, ReturnFromRoute);
	}

	void ReturnFromShowingMovementTiles()
	{
		TileGrid.Instance.RouteAICharacterToTile(owningCharacter, new List<Tile>(route.Item1), ReturnFromRoute);
	}

	void ReturnFromRoute()
	{
		foreach (Tile t in litRouteTiles)
		{
			t.HideOverlay(Tile.OverlayType.PossibleMovement);
		}

		Attack();
	}

	void Attack()
	{
		targetTiles = TemplateLibrary.GetAdjacentCharacterTarget(owningCharacter, null);
		if (targetTiles == null)
		{
			ShowNoTarget(owningCharacter.transform.position);
			ApplyThrowingSand();
			Finish();
			return;
		}

		AnimationController.Instance.ShowTiles(targetTiles, Tile.OverlayType.PossibleAttck, ReturnFromShowingAttackTiles);
	}

	void ReturnFromShowingAttackTiles()
	{
		owningCharacter.SetFacing(TileGrid.Instance.GetFacingDirection(owningCharacter, targetTiles[0].character));
		ActionController.Instance.PlayAttackAnimation(owningCharacter, null, () =>
		{
			foreach (Tile t in targetTiles)
			{
				if (t.character != null && t.character.hero)
				{
					ActionController.Instance.AttackCharacter(t.character, owningCharacter, new AttackProfile(1, 6, 0));
				}
				t.HideOverlay(Tile.OverlayType.PossibleAttck);
			}

			ApplyThrowingSand();
			Finish();
		});
	}

	void ApplyThrowingSand()
	{
		if (moved)
		{
			return;
		}

		owningCharacter.AddStatusEffect(typeof(ThrowingSand), null);
	}

	public static List<CardInstruction> GetCardInstructions(CardScriptableObject scriptableObject)
	{
		DisplayGrid.Instance.Clear(11, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction($"Move to the closest enemy and attack for 1d6 damage"));
		instructions.Add(new CardInstruction("If no movement was needed, apply<u>Throwing Sand</u> to the attacker"));
		DisplayGrid.Instance.Show();
		return instructions;
	}
}
