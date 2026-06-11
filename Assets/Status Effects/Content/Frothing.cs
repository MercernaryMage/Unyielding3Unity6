using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frothing : StatusEffect
{
	public override void OnDamageDealt(DamageDealtMessage damageDealtMessage)
	{
		if (damageDealtMessage.attacker == character)
		{
			damageDealtMessage.defender.AddStatusEffect(typeof(Slobbered), null);
		}
	}

	public override void OnHeroDowned(HeroDownedMessage heroDownedMessage)
	{
		Destroy(this);
	}

	public override string GetDisplayName()
	{
		return "Frothing";
	}

	public override string GetEffectText()
	{
		return "All damage causes Slobbered until a character is downed.";
	}
}
