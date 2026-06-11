using System.Collections.Generic;
using UnityEngine;

public class Defend : StatusEffect
{
    int turnsRemaining = 3;

    public override void CharacterEndTurn(CharacterEndTurnMessage characterEndTurnMessage)
    {
        if (characterEndTurnMessage.character != character)
        {
            return;
        }
        turnsRemaining--;
        if (turnsRemaining <= 0)
        {
            Destroy(this);
        }
    }

    public override void OnPreDamageDealt(PreDamageDealtMessage preDamageDealtMessage)
    {
        if (preDamageDealtMessage.defender != character)
        {
            return;
        }
        preDamageDealtMessage.hasResistance = true;
    }

    public override string GetDisplayName()
    {
        return "Defend";
    }

    public override string GetEffectText()
    {
        return "Movement is halved. Takes half damage from all sources.";
    }

	public static List<CardInstruction> GetCardInstructions()
	{
		DisplayGrid.Instance.Clear(11, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction($"Movement is halved. Takes half damage from all sources."));
		instructions.Add(new CardInstruction($"Play this characters next card"));

		return instructions;
	}
}
