using System.Collections.Generic;
using UnityEngine;

public class Imposed : StatusEffect
{
    public override void OnCharacterAttacking(CharacterAttackingMessage characterAttackingMessage)
    {
        if (characterAttackingMessage.attacker != character)
        {
            return;
        }
      
        if (characterAttackingMessage.ranged)
		{
			characterAttackingMessage.accuracy -= 1;
			characterAttackingMessage.AddToAccuracyString($"-1 ({GetExplanationName()})");
		}
    }

	public override void OnCharacterFinishedMoving(CharacterFinishedMovingMessage characterFinishedMovingMessage)
	{
		if (characterFinishedMovingMessage.movingCharacter != character)
		{
			return;
		}
		List<Character> characters = TileGrid.Instance.GetAllAdjacentCharcters(character);

		foreach (Character c in characters)
		{
			if (c.hero != character.hero)
			{
				return;
			}
		}

		Destroy(this);
	}

	public override string GetExplanationName()
    {
        return "Imposed";
    }


	public static List<CardInstruction> GetCardInstructions()
	{
		DisplayGrid.Instance.Clear(11, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction($"Adjacent enemy is giving -1 accuracy to ranged attacks."));

		return instructions;
	}

	public override bool ShowStatusEffectFloatingCombatMessage()
	{
		return false;
	}
}
