using System.Collections.Generic;
using UnityEngine;

public class StealBuffsTrait : Trait
{
	public override void DamageDealt(DamageDealtMessage message)
	{
		if (message.attacker != character)
		{
			return;
		}
		int stolenArmor = message.defender.armor;
		if (stolenArmor <= 0)
		{
			return;
		}
		message.defender.armor = 0;
		character.armor += stolenArmor;
	}
}
