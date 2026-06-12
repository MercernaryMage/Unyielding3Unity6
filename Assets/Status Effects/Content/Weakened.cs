using UnityEngine;

public class Weakened : StatusEffect
{
	public int value = 3;

	public override void OnPreDamageDealt(PreDamageDealtMessage preDamageDealtMessage)
	{
		if (preDamageDealtMessage.attacker != character)
		{
			return;
		}

		preDamageDealtMessage.damage -= value;
		if (preDamageDealtMessage.results != null)
		{
			preDamageDealtMessage.results.outString += $" - {value} (weakened)";
		}
		value--;
		if (value <= 0)
		{
			Destroy(this);
		}
	}

	public override string GetDisplayName()
	{
		return "Weakened";
	}

	public override string GetEffectText()
	{
		return $"Deals {value} less damage. Reduces by 1 each time it applies, removed at 0.";
	}
}
