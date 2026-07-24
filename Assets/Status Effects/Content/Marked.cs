using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marked : StatusEffect
{
	public override string GetExplanationName()
	{
		return "Marked";
	}

	public override void OnCharacterAttacking(CharacterAttackingMessage characterAttackingMessage)
	{
		if (characterAttackingMessage.defender == character)
		{
			characterAttackingMessage.accuracy += 1;
			characterAttackingMessage.AddToAccuracyString($"+1 ({GetExplanationName()})");
			Destroy(this);
		}
	}

}
