using System.Collections.Generic;
using UnityEngine;

public class ThrowingSand : StatusEffect
{
    int marksApplied = 0;

    public override void OnCharacterHit(CharacterHitMessage characterHitMessage)
    {
        if (characterHitMessage.attacker != character)
        {
            return;
        }
        if (characterHitMessage.defender.GetComponent<Marked>() != null)
        {
            return;
        }
        characterHitMessage.defender.AddStatusEffect(typeof(Marked), null);
        marksApplied++;
        if (marksApplied >= 3)
        {
            Destroy(this);
        }
    }

    public override string GetStatusEffectKey()
    {
        return "Throwing Sand";
    }


	public static List<CardInstruction> GetCardInstructions()
	{
		DisplayGrid.Instance.Clear(11, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction(ExplanationItemRepository.Instance.GetExplanation("Throwing Sand").explanationContent));

		return instructions;
	}
}
