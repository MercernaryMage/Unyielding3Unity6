using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LungeN : ReactionBase
{
	Pounce runningPounce;

	public override void Execute()
	{
		owningCharacter.SetFacing(TileGrid.Instance.GetFacingDirection(owningCharacter, attackingCharacter));
		SkillCheckController.Instance.Set(SkillCheckController.SkillCheckType.Cunning, GetIntValue("Difficulty"), attackingCharacter, ReturnFromCheck);
	}

	public void ReturnFromCheck(bool checkResult)
	{
		if (checkResult == false)
		{
			runningPounce = new Pounce();
			runningPounce.Execute(owningCharacter, attackingCharacter, ReturnFromPounce);
			return;
		}
		BattleController.ReturnControlToPlayer();
		return;
	}

	public void ReturnFromPounce()
	{
		runningPounce = null;
		BattleController.ReturnControlToPlayer();

	}

	public static List<CardInstruction> GetCardInstructions(CardScriptableObject scriptableObject)
	{
		DisplayGrid.Instance.Clear(12, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction("Face attacker"));
		instructions.Add(new CardInstruction($"Perform a cunning check diff:{scriptableObject.GetTagIntValue("Difficulty")}"));
		instructions.Add(new CardInstruction("On failure pounce foward for 1d6 damage"));
		return instructions;
	}
}
