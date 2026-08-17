using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Downed : StatusEffect
{
	public override void CharacterEndTurn(CharacterEndTurnMessage characterEndTurnMessage)
	{
		if (characterEndTurnMessage.character == character)
		{
			ClearEffect();
			Destroy(this);
		}
	}

	public void ClearEffect()
	{
		character.currentHP = character.maxHP;
	}

	public override string GetExplanationName()
	{
		return "Downed";
	}

}
