using System.Collections.Generic;
using UnityEngine;

public class FightingDirty : StatusEffect
{
    int marksApplied = 0;

    public override void OnDamageDealt(DamageDealtMessage damageDealtMessage)
    {
        if (damageDealtMessage.attacker != character)
        {
            return;
        }
        if (damageDealtMessage.defender.GetComponent<Marked>() != null)
        {
            return;
        }
        damageDealtMessage.defender.AddStatusEffect(typeof(Marked), null);
        marksApplied++;
        if (marksApplied >= 3)
        {
            Destroy(this);
        }
    }

    public override string GetDisplayName()
    {
        return "Fighting Dirty";
    }

    public override string GetEffectText()
    {
        return "Attacks apply Marked to targets that don't already have it. Expires after 3 applications.";
    }

	public static List<CardInstruction> GetCardInstructions()
	{
		DisplayGrid.Instance.Clear(11, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction($"Attacks apply Marked to targets that don't already have it. Expires after 3 applications."));
		instructions.Add(new CardInstruction($"Play this characters next card"));

		return instructions;
	}
}
