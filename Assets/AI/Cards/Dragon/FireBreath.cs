using System;
using System.Collections.Generic;
using UnityEngine;

public class FireBreath : Card
{
	TemplateLibrary.TilesAndDirection tilesAndDirection;

	public override void Execute()
	{
		tilesAndDirection = TemplateLibrary.Instance.ConeTargeting(owningCharacter, 30);
		if (tilesAndDirection == null)
		{
			Finish();
			return;
		}
		Util.FilterToLOS(tilesAndDirection, owningCharacter);
		if (tilesAndDirection.tiles.Count == 0)
		{
			Finish();
			return;
		}
		owningCharacter.SetFacing(tilesAndDirection.direction);
		AnimationController.Instance.ShowTiles(tilesAndDirection.tiles, Tile.OverlayType.PossibleAttck, ReturnFromShowingAttackTiles);
	}

	void ReturnFromShowingAttackTiles()
	{
		ActionController.Instance.PlayAttackAnimation(owningCharacter, null, () =>
		{
			List<Character> hitCharacters = new List<Character>();
			foreach (Tile t in tilesAndDirection.tiles)
			{
				if (t.character != null && t.character.hero && !hitCharacters.Contains(t.character))
				{
					hitCharacters.Add(t.character);
					ActionController.AttackProfile profile = new ActionController.AttackProfile(1, 6, 3);
					profile.damageType = ActionPattern.DamageType.Burning;
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
		instructions.Add(new CardInstruction("Breathe fire in a large cone"));
		instructions.Add(new CardInstruction());
		instructions.Add(new CardInstruction("Deal 1d6+3 burning damage to each target"));
		DisplayGrid.Instance.Show();
		return instructions;
	}
}
