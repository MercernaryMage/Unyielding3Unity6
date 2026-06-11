using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlinkN : ReactionBase
{
	public override void Execute()
	{
		owningCharacter.SetFacing(TileGrid.Instance.GetFacingDirection(owningCharacter, attackingCharacter));
		SkillCheckController.Instance.Set(SkillCheckController.SkillCheckType.Prowess, GetIntValue("Difficulty"), attackingCharacter, ReturnFromCheck);
	}

	public void ReturnFromCheck(bool checkResult)
	{
		if (checkResult == false)
		{
			HowlAndGrowl.Growl(owningCharacter, 1);
		}
		else
		{
			owningCharacter.toughness -= 1;

		}
		BattleController.ReturnControlToPlayer();
		return;
	}

	public static List<CardInstruction> GetCardInstructions(CardScriptableObject scriptableObject)
	{
		DisplayGrid.Instance.Clear(12, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction($"Perform a prowess check diff:{scriptableObject.GetTagIntValue("Difficulty")}"));
		instructions.Add(new CardInstruction("On success reduce monster toughness by 1"));
		instructions.Add(new CardInstruction("On failure Growl"));
		return instructions;
	}
}
