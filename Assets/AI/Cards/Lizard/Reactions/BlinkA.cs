using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlinkA : ReactionBase
{
	Pounce runningPounce;

	public override void Execute()
	{
		
		owningCharacter.SetFacing(TileGrid.Instance.GetFacingDirection(owningCharacter, attackingCharacter));
		HowlAndGrowl.Howl(owningCharacter, 1);
		BattleController.Instance.HealCharacter(owningCharacter, 2);
		BattleController.ReturnControlToPlayer();
	}

	public static List<CardInstruction> GetCardInstructions(CardScriptableObject scriptableObject)
	{
		DisplayGrid.Instance.Clear(12, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction("Howl"));
		instructions.Add(new CardInstruction("Heal 2"));
		return instructions;
	}
}
