using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marked : StatusEffect
{
	public override string GetDisplayName()
	{
		return "Marked";
	}

	public override void OnCharacterAttacking(CharacterAttackingMessage characterAttackingMessage)
	{
		if (characterAttackingMessage.defender == character)
		{
			characterAttackingMessage.accuracy += 1;
			characterAttackingMessage.AddToAccuracyString($"+1 ({GetDisplayName()})");
			Destroy(this);
		}
	}

	public override string GetEffectText()
	{
		return "Attacking this target consumes this effect to add an accuracy.";
	}
}
