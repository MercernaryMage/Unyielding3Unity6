using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inaccuracy : StatusEffect
{
	public override string GetDisplayName()
	{
		return "Inaccuracy";
	}

	public override void OnCharacterAttacking(CharacterAttackingMessage characterAttackingMessage)
	{
		if (characterAttackingMessage.attacker == character)
		{
			characterAttackingMessage.accuracy -= 1;
			characterAttackingMessage.AddToAccuracyString($"-1 ({GetDisplayName()})");
		}
	}

	public override void OnCharacterMiss(CharacterMissMessage characterMissMessage)
	{
		if (characterMissMessage.attacker == character)
		{
			Destroy(this);
		}
	}

	public override string GetEffectText()
	{
		return "Subtracts an accuracy.  Removed the first time this character misses.";
	}
}
