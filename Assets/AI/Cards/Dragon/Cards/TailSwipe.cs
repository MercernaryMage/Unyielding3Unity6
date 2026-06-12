using System;
using System.Collections.Generic;
using UnityEngine;

public class TailSwipe : Card
{
	List<Tile> attackTiles;
	bool backAttack;

	public override void Execute()
	{
		Direction backDirection = TileGrid.GetOppositeFacing(owningCharacter.facing);
		List<Tile> backTiles = TemplateLibrary.Instance.GetTilesInMatchedSizeCardinalDirection(
			owningCharacter, 1, (int)backDirection);

		backAttack = false;
		foreach (Tile t in backTiles)
		{
			if (t.character != null && t.character.hero)
			{
				backAttack = true;
				break;
			}
		}

		if (backAttack)
		{
			attackTiles = backTiles;
		}
		else
		{
			Tile characterTile = TileGrid.Instance.FindCharacter(owningCharacter)[0];
			Character closestHero = TileGrid.Instance.FindClosestHero(characterTile);
			if (closestHero == null)
			{
				Finish();
				return;
			}
			attackTiles = TileGrid.Instance.FindCharacter(closestHero);
			owningCharacter.SetFacing(TileGrid.Instance.GetFacingDirection(owningCharacter, closestHero));
		}

		AnimationController.Instance.ShowTiles(attackTiles, Tile.OverlayType.PossibleAttck, ReturnFromShowingAttackTiles);
	}

	void ReturnFromShowingAttackTiles()
	{
		ActionController.Instance.PlayAttackAnimation(owningCharacter, null, () =>
		{
			List<Character> hitCharacters = new List<Character>();
			foreach (Tile t in attackTiles)
			{
				if (t.character != null && t.character.hero && !hitCharacters.Contains(t.character))
				{
					hitCharacters.Add(t.character);
					ActionController.AttackProfile profile = backAttack
						? new ActionController.AttackProfile(1, 3, 0)
						: new ActionController.AttackProfile(1, 6, 3);
					ActionController.Instance.AttackCharacter(t.character, owningCharacter, profile);
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
		instructions.Add(new CardInstruction("If enemies are adjacent and behind,"));
		instructions.Add(new CardInstruction("attack them for 1d3 damage"));
		instructions.Add(new CardInstruction());
		instructions.Add(new CardInstruction("Otherwise, attack closest enemy for 1d6+3 damage"));
		DisplayGrid.Instance.Show();
		return instructions;
	}
}
