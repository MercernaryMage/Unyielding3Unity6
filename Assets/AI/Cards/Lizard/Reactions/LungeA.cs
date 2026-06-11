using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LungeA : ReactionBase
{
	Pounce runningPounce;

	public override void Execute()
	{
		owningCharacter.SetFacing(TileGrid.Instance.GetFacingDirection(owningCharacter, attackingCharacter));
		runningPounce = new Pounce();
		runningPounce.Execute(owningCharacter, attackingCharacter, ReturnFromPounce);
	}

	public void ReturnFromPounce()
	{
		runningPounce = null;
		BattleController.Instance.HealCharacter(owningCharacter, 2);
		BattleController.ReturnControlToPlayer();
	}

	public static List<CardInstruction> GetCardInstructions(CardScriptableObject scriptableObject)
	{
		DisplayGrid.Instance.Clear(12, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction("Face attacker"));
		instructions.Add(new CardInstruction("Pounce towards for 1d6 damage"));
		return instructions;
	}
}
