using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Downed : StatusEffect
{
	bool readyToClear = false;
	public override void CharacterStartTurn(CharacterStartTurnMessage characterStartTurnMessage)
	{
		readyToClear = true;
	}

	public override void CharacterEndTurn(CharacterEndTurnMessage characterEndTurnMessage)
	{
		if (characterEndTurnMessage.character == character && readyToClear)
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
