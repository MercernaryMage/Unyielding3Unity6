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

	public override string GetDisplayName()
	{
		return "Stunned";
	}

	public override string GetEffectText()
	{
		return "Cannot take actions or move.  Will be removed at the end of the units turn.";
	}
}
