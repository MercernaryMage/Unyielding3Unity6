using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poison : StatusEffect
{
	public int value;

	public override string GetExplanationName()
	{
		return "Poison";
	}

	public override void OnCharacterCritCheck(CharacterCritCheckMessage characterCritCheckMessage)
	{
		if (characterCritCheckMessage.defender == character)
		{
			characterCritCheckMessage.critThreshold -= value;
		}
	}

	public override void OnCharacterCrit(CharacterCritMessage characterCritMessage)
	{
		if (characterCritMessage.defender == character)
		{
			value--;
			if (value <= 0)
			{
				Destroy(this);
			}
		}
	}

}
