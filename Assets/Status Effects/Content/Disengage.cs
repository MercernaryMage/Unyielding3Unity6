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

	public override string GetDisplayName()
	{
		return "Disengage";
	}

	public override string GetEffectText()
	{
		return "This character does not provoke attacks of opportunity when moving.";
	}

	public static List<CardInstruction> GetCardInstructions()
	{
		DisplayGrid.Instance.Clear(11, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction($"This character does not provoke attacks of opportunity when moving."));
		instructions.Add(new CardInstruction($"Play this characters next card"));

		return instructions;
	}
}
