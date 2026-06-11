using System;
using System.Collections.Generic;
using UnityEngine;
using static ActionController;

public class Impale : Card
{
	Character target;
	List<Tile> targetTiles;
	Tuple<List<Tile>, Tile> route;

	public override void Execute()
	{
		Dictionary<Character, Tuple<List<Tile>, Tile>> routes = RouteToAllClosestCharacters(true);

		route = null;
		int highestArmor = -1;

		foreach (KeyValuePair<Character, Tuple<List<Tile>, Tile>> pair in routes)
		{
			Character hero = pair.Key;
			if (hero.armor > highestArmor)
			{
				highestArmor = hero.armor;
				target = hero;
				route = pair.Value;
			}
		}

		if (route == null)
		{
			Finish();
			return;
		}

		Util.ShortenPathToMaxRange(route, owningCharacter.characterDefinition.movement + 1);
		AnimationController.Instance.ShowTiles(route.Item1, Tile.OverlayType.PossibleMovement, ReturnFromShowingMovementTiles, ReturnFromRoute);
	}

	void ReturnFromShowingMovementTiles()
	{
		TileGrid.Instance.RouteCharacterToTile(owningCharacter, new List<Tile>(route.Item1), ReturnFromRoute);
	}

	void ReturnFromRoute()
	{
		foreach (Tile t in route.Item1)
		{
			t.HideOverlay(Tile.OverlayType.PossibleMovement);
		}

		targetTiles = TemplateLibrary.GetAdjacentCharacterTarget(owningCharacter, null);
		if (targetTiles == null)
		{
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
					AttackProfile attackProfile = new AttackProfile(1, 6, 0);
					attackProfile.preDamageAction = () =>
					{
						t.character.armor = 0;
					};
					ActionController.Instance.AttackCharacter(t.character, owningCharacter, new AttackProfile(1, 6, 0));
				}
				t.HideOverlay(Tile.OverlayType.PossibleAttck);
			}

			Finish();
		});
	}

	public static List<CardInstruction> GetCardInstructions(CardScriptableObject scriptableObject)
	{
		DisplayGrid.Instance.Clear(11, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction("Move toward the hero with the most armor"));
		instructions.Add(new CardInstruction("Set their armor to 0, then attack for 1d6"));
		DisplayGrid.Instance.Show();
		return instructions;
	}
}
