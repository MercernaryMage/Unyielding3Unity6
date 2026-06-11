using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShieldGlare : Card
{
	TemplateLibrary.TilesAndDirection tilesAndDirection;

	public override void Execute()
	{
		tilesAndDirection = TemplateLibrary.Instance.ConeTargeting(owningCharacter, GetIntValue("range"));
		if (tilesAndDirection == null)
		{
			FloatingCombatNumberController.Instance.QueueFloatingCombatNumber(owningCharacter, "No target");
			Util.BringCardToTopOfDeck(owningCharacter, "ShieldRush");
			AnimationController.Instance.DelayedCallback(3f, () => AIController.Instance.TakeTurn(owningCharacter));
			return;
		}
		Util.FilterToLOS(tilesAndDirection, owningCharacter);
		AnimationController.Instance.ShowTiles(tilesAndDirection.tiles, Tile.OverlayType.PossibleAttck, ReturnFromShowingAttackTiles);
	}

	public void ReturnFromShowingAttackTiles()
	{
		owningCharacter.SetFacing(tilesAndDirection.direction);
		ActionController.Instance.PlayAttackAnimation(owningCharacter, null, () =>
		{
			foreach (Tile t in tilesAndDirection.tiles)
			{
				if (t.character && t.character.hero)
				{
					ActionController.Instance.AttackCharacter(t.character, owningCharacter, new ActionController.AttackProfile(1, 6, 0));
					t.character.AddStatusEffect(typeof(Inaccuracy), null);
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
		instructions.Add(new CardInstruction("Hit enemies in cone pattern"));
		instructions.Add(new CardInstruction());
		instructions.Add(new CardInstruction("Deal 1d6 damage to each target"));
		instructions.Add(new CardInstruction());
		instructions.Add(new CardInstruction("No targets: play Shield Rush instead"));

		DisplayGrid.Instance.Add(DisplayGrid.DisplayGridObject.Size1Enemy, DisplayGrid.DisplayGridDirection.South, 5, 4);
		DisplayGrid.Instance.Add(DisplayGrid.DisplayGridObject.EffectedTile, new List<Tuple<int, int>>()
		{
			new Tuple<int, int>(6, 3),
			new Tuple<int, int>(5, 3),
			new Tuple<int, int>(4, 3),
		});
		DisplayGrid.Instance.Show();

		return instructions;
	}
}
