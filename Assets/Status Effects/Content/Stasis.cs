using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stasis : Downed
{
	public override void CharacterEndTurn(CharacterEndTurnMessage characterEndTurnMessage)
	{
// 		if (characterEndTurnMessage.character == character)
// 		{
// 			Destroy(this);
// 		}
	}

	public override string GetDisplayName()
	{
		return "Stasis";
	}

	public override string GetEffectText()
	{
		return "Cannot take actions, move, or be effected by things.";
	}
}
