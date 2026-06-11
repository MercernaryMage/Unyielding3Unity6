using System;
using System.Collections.Generic;
using UnityEngine;

public class WingBuffet : Card
{
	TemplateLibrary.TilesAndDirection tilesAndDirection;

	public override void Execute()
	{
		tilesAndDirection = TemplateLibrary.Instance.GetMostFacingDirection(owningCharacter);
		if (tilesAndDirection == null)
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
					ActionController.Instance.AttackCharacter(t.character, owningCharacter, new ActionController.AttackProfile(1, 6, 0));
					ActionController.Instance.KnockBack(t.character, TileGrid.Instance.FindCharacter(owningCharacter), 1);
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
		instructions.Add(new CardInstruction("Face the direction with the most enemies"));
		instructions.Add(new CardInstruction());
		instructions.Add(new CardInstruction("Deal 1d6 damage to each target in that facing"));
		instructions.Add(new CardInstruction("Knockback 1 on hit"));
		DisplayGrid.Instance.Show();
		return instructions;
	}
}
