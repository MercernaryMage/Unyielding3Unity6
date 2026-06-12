using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WoundA : ReactionBase
{
	public override void Execute()
	{
		owningCharacter.SetFacing(TileGrid.Instance.GetFacingDirection(owningCharacter, attackingCharacter));
		SkillCheckController.Instance.Set(SkillCheckController.SkillCheckType.Prowess, GetIntValue("Difficulty"), attackingCharacter, ReturnFromCheck);
	}

	public void ReturnFromCheck(bool checkResult)
	{
		if (checkResult)
		{
			StatusEffect.StatusEffectInitData initData = new StatusEffect.StatusEffectInitData();
			initData.magnitude = 1;
			owningCharacter.AddStatusEffect(typeof(Injured), initData);
		}
		BattleController.ReturnControlToPlayer();
	}

	public static List<CardInstruction> GetCardInstructions(CardScriptableObject scriptableObject)
	{
		DisplayGrid.Instance.Clear(12, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction($"Perform a prowess check diff:{scriptableObject.GetTagIntValue("Difficulty")}"));
		instructions.Add(new CardInstruction("On success become Injured"));
		return instructions;
	}
}
