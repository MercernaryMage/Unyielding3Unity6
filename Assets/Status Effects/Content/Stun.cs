using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stun : StatusEffect
{
	public override void CharacterEndTurn(CharacterEndTurnMessage characterEndTurnMessage)
	{
		if (characterEndTurnMessage.character == character)
		{
			Destroy(this);
		}
	}

	public override string GetExplanationName()
	{
		return "Stunned";
	}

}
