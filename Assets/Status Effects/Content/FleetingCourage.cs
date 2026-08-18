using System.Collections.Generic;
using UnityEngine;

public class FleetingCourage : StatusEffect
{
	public int value = 1;

	public override void OnPreDamageDealt(PreDamageDealtMessage preDamageDealtMessage)
	{
		if (preDamageDealtMessage.attacker != character || preDamageDealtMessage.attacker == preDamageDealtMessage.defender)
		{
			return;
		}

		preDamageDealtMessage.damage += value;
		if (preDamageDealtMessage.results != null)
		{
			preDamageDealtMessage.results.outString += $" + {value} (fleeting courage)";
		}
	}

	public override void OnDamageDealt(DamageDealtMessage damageDealtMessage)
	{
		if (damageDealtMessage.defender != character)
		{
			return;
		}

		Destroy(this);
	}

	public override void OnHeroDowned(HeroDownedMessage heroDownedMessage)
	{
		if (heroDownedMessage.downedCharacter != character)
		{
			return;
		}

		Destroy(this);
	}

	public override string GetExplanationName()
	{
		return "Fleeting Courage";
	}

	public static List<CardInstruction> GetCardInstructions()
	{
		DisplayGrid.Instance.Clear(11, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction($"Attacks deal 1 additional damage. Removed when this character takes damage."));
		return instructions;
	}
}
