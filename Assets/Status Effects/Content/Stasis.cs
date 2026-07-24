using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stasis : Downed
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
		return "Stasis";
	}

}
