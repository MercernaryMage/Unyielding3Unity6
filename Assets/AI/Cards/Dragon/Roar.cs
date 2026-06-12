using System;
using System.Collections.Generic;
using UnityEngine;

public class Roar : Card
{
	List<Tile> targetTiles;

	public override void Execute()
	{
		List<Tile> characterTiles = TileGrid.Instance.FindCharacter(owningCharacter);

		switch (owningCharacter.facing)
		{
			case Direction.North:
				targetTiles = TemplateLibrary.Instance.GetNorthFacingTiles(characterTiles);
				break;
			case Direction.East:
				targetTiles = TemplateLibrary.Instance.GetEastFacingTiles(characterTiles);
				break;
			case Direction.South:
				targetTiles = TemplateLibrary.Instance.GetSouthFacingTiles(characterTiles);
				break;
			default:
				targetTiles = TemplateLibrary.Instance.GetWestFacingTiles(characterTiles);
				break;
		}

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
