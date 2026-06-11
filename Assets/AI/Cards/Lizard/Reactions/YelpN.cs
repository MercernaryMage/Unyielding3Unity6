using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class YelpN : ReactionBase
{
	List<Tile> tiles;
	public override void Execute()
	{		
		owningCharacter.SetFacing(TileGrid.Instance.GetFacingDirection(owningCharacter, attackingCharacter));
		if (!TileGrid.Instance.CharactersAreAdjacent(owningCharacter, attackingCharacter))
		{
			FloatingCombatNumberController.Instance.QueueFloatingCombatNumber(owningCharacter, "out of range");
			BattleController.ReturnControlToPlayer();
			return;
		}
		SkillCheckController.Instance.Set(SkillCheckController.SkillCheckType.Prowess, GetIntValue("Difficulty"), attackingCharacter, ReturnFromCheck);
	}

	public void ReturnFromCheck(bool result)
	{
		if (result == true)
		{
			BattleController.ReturnControlToPlayer();
			return;
		}
		tiles = TileGrid.Instance.FindCharacter(attackingCharacter);
		AnimationController.Instance.ShowTiles(tiles, Tile.OverlayType.PossibleAttck, ReturnFromShowingAttackTiles);
	}

	public void ReturnFromShowingAttackTiles()
	{
		ActionController.Instance.PlayAttackAnimation(owningCharacter, null, () => 
		{
			ActionController.Instance.AttackCharacter(attackingCharacter, owningCharacter, new ActionController.AttackProfile(1, 6, 0));
			foreach (Tile t in tiles)
			{
				t.HideOverlay(Tile.OverlayType.PossibleAttck);
			}
			BattleController.ReturnControlToPlayer();
		});
	}

	public static List<CardInstruction> GetCardInstructions(CardScriptableObject scriptableObject)
	{
		DisplayGrid.Instance.Clear(12, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction("Face attacker"));
		instructions.Add(new CardInstruction($"If attacker is adjacent perform a prowess check diff:{scriptableObject.GetTagIntValue("Difficulty")}"));
		instructions.Add(new CardInstruction("On failure counterattack the attacker for 1d6 damage"));
		return instructions;
	}
}
