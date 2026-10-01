using System.Collections.Generic;
using UnityEngine;

public class MarkedForDeath : StatusEffect
{
	public Character causingCharacter;
	public int value = 5;

	public override void OnPreDamageDealt(PreDamageDealtMessage preDamageDealtMessage)
	{
		if (preDamageDealtMessage.defender != character)
		{
			return;
		}
		if (preDamageDealtMessage.attacker != causingCharacter)
		{
			return;
		}
		if (preDamageDealtMessage.damage <= 0)
		{
			return;
		}

		preDamageDealtMessage.damage += value;
		if (preDamageDealtMessage.results != null)
		{
			preDamageDealtMessage.results.outString += $" + {value} ({GetExplanationName()})";
		}
		Destroy(this);
	}

	public override string GetExplanationName()
	{
		return "Marked For Death";
	}

	public static List<CardInstruction> GetCardInstructions()
	{
		DisplayGrid.Instance.Clear(11, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction($"The next time the character that applied this effect damages this target, it deals 5 additional damage and the effect is consumed."));

		return instructions;
	}
}
