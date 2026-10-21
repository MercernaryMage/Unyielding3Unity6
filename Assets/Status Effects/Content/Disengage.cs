using System.Collections.Generic;
using UnityEngine;

public class Disengage : StatusEffect
{
	public override void OnCharacterFinishedMoving(CharacterFinishedMovingMessage message)
	{
		if (message.movingCharacter == character)
		{
			Destroy(this);
		}
	}

	public override string GetStatusEffectKey()
	{
		return "Disengage";
	}


	public static List<CardInstruction> GetCardInstructions()
	{
		DisplayGrid.Instance.Clear(11, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction(ExplanationItemRepository.Instance.GetExplanation("Disengage").explanationContent));
		instructions.Add(new CardInstruction($"Play this characters next card"));

		return instructions;
	}
}
