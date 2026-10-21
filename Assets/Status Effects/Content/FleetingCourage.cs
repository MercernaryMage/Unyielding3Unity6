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

	public override string GetStatusEffectKey()
	{
		return "Fleeting Courage";
	}

	public override string GetIconName()
	{
		return "status_symbol_courage";
	}

	public static List<CardInstruction> GetCardInstructions()
	{
		DisplayGrid.Instance.Clear(11, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction(ExplanationItemRepository.Instance.GetExplanation("Fleeting Courage").explanationContent));
		return instructions;
	}
}
