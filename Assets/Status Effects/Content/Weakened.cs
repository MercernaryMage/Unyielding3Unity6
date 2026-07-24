using UnityEngine;

public class Weakened : StatusEffect
{
	public int value = 3;

	public override void OnPreDamageDealt(PreDamageDealtMessage preDamageDealtMessage)
	{
		if (preDamageDealtMessage.attacker != character || preDamageDealtMessage.attacker == preDamageDealtMessage.defender)
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

	public override string GetExplanationName()
	{
		return "Weakened";
	}

}
