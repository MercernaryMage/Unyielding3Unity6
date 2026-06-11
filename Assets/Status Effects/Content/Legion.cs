using System.Collections.Generic;
using UnityEngine;

public class Legion : StatusEffect
{
    public override void OnCharacterAttacking(CharacterAttackingMessage characterAttackingMessage)
    {
        if (characterAttackingMessage.attacker != character)
        {
            return;
        }
        foreach (Character ally in BattleController.Instance.GetAllies(character))
        {
            if (ally == character)
            {
                continue;
            }
            if (TileGrid.Instance.CharactersAreAdjacent(character, ally))
            {
                characterAttackingMessage.accuracy += 1;
				characterAttackingMessage.AddToAccuracyString($"+1 ({GetDisplayName()})");
				return;
            }
        }
    }

    public override string GetDisplayName()
    {
        return "Legion";
    }

    public override string GetEffectText()
    {
        return "Attacks gain +1 accuracy while adjacent to an ally.";
    }

	public static List<CardInstruction> GetCardInstructions()
	{
		DisplayGrid.Instance.Clear(11, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction($"Attacks gain +1 accuracy while adjacent to an ally."));
		instructions.Add(new CardInstruction($"Play this characters next card"));

		return instructions;
	}
}
