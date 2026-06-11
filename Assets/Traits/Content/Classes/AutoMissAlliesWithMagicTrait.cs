using System.Collections.Generic;
using UnityEngine;

public class AutoMissAlliesWithMagicTrait : Trait
{
	public override void CharacterAttack(CharacterAttackingMessage message)
	{
		if (message.attacker.hero == message.defender.hero)
		{
			if (message.pattern.physical == false)
			{
				message.autoMiss = true;
			}
		}
	}
}
