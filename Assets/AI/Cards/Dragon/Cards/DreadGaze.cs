using System;
using System.Collections.Generic;
using UnityEngine;

public class DreadGaze : Card
{
	List<Tile> targetTiles;

	public override void Execute()
	{
		List<Tile> characterTiles = TileGrid.Instance.FindCharacter(owningCharacter);
		targetTiles = TemplateLibrary.Instance.GetFacingTiles(characterTiles, owningCharacter.facing);

		AnimationController.Instance.ShowTiles(targetTiles, Tile.OverlayType.PossibleAttck, ReturnFromShowingAttackTiles);
	}

	void ReturnFromShowingAttackTiles()
	{
		ActionController.Instance.PlayAttackAnimation(owningCharacter, null, () =>
		{
			List<Character> hitCharacters = new List<Character>();
			foreach (Tile t in targetTiles)
			{
				if (t.character != null && t.character.hero && !hitCharacters.Contains(t.character))
				{
					hitCharacters.Add(t.character);
					t.character.AddStatusEffect(typeof(Freightened), null);
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
		instructions.Add(new CardInstruction("Apply Freightened to each enemy in front of you"));
		DisplayGrid.Instance.Show();
		return instructions;
	}
}
