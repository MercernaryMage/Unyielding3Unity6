using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inaccuracy : StatusEffect
{
	int stack = 1;

	public override string GetExplanationName()
	{
		return "Inaccuracy";
	}

	public override void OnCharacterAttacking(CharacterAttackingMessage characterAttackingMessage)
	{
		if (characterAttackingMessage.attacker == character)
		{
			characterAttackingMessage.accuracy -= stack;
			characterAttackingMessage.AddToAccuracyString($"-{stack} ({GetExplanationName()})");
		}
	}

	public override void OnCharacterMiss(CharacterMissMessage characterMissMessage)
	{
		if (characterMissMessage.attacker == character)
		{
			Destroy(this);
		}
	}

	public override void DoStack(StatusEffectInitData data)
	{
		++stack;
		Debug.Log(stack);
	}
}
