using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inaccuracy : StatusEffect
{
	public override string GetExplanationName()
	{
		return "Inaccuracy";
	}

	public override void OnCharacterAttacking(CharacterAttackingMessage characterAttackingMessage)
	{
		if (characterAttackingMessage.attacker == character)
		{
			characterAttackingMessage.accuracy -= 1;
			characterAttackingMessage.AddToAccuracyString($"-1 ({GetExplanationName()})");
		}
	}

	public override void OnCharacterMiss(CharacterMissMessage characterMissMessage)
	{
		if (characterMissMessage.attacker == character)
		{
			Destroy(this);
		}
	}

}
