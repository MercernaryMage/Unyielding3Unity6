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
		owningCharacter.SetFacing(tilesAndDirection.direction);
		List<Character> targets = new List<Character>();
		foreach (Tile t in tilesAndDirection.tiles)
		{
			if (t.character != null && t.character.hero && !targets.Contains(t.character))
			{
				targets.Add(t.character);
			}
		}
		ActionController.Instance.PlayAdvancedAttackAnimation(owningCharacter, targets, cardScriptableObject.effects[0], null, () =>
		{
			foreach (Character t in targets)
			{
				ActionController.AttackProfile profile = new ActionController.AttackProfile(1, 6, 3);
				profile.damageType = ActionPattern.DamageType.Burning;
				ActionController.Instance.AttackCharacter(t, owningCharacter, profile);
			}
			foreach (Tile t in tilesAndDirection.tiles)
			{
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
		instructions.Add(new CardInstruction("Deal 1d6+3 <u>Burning</u> damage to each target"));
		DisplayGrid.Instance.Show();
		return instructions;
	}
}
