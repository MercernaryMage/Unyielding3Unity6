using System.Collections.Generic;
using UnityEngine;

public class Sapped : StatusEffect
{
	public override void Start()
	{
		base.Start();
		LimitMovement();
	}

	public override void DoStack(StatusEffectInitData data)
	{
		LimitMovement();
	}

	public override void CharacterStartTurn(CharacterStartTurnMessage characterStartTurnMessage)
	{
		if (characterStartTurnMessage.character != character)
		{
			return;
		}
		LimitMovement();
	}

	public override void CharacterEndTurn(CharacterEndTurnMessage characterEndTurnMessage)
	{
		if (characterEndTurnMessage.character != character)
		{
			return;
		}
		if (character.actionCount >= 3)
		{
			Destroy(this);
		}
	}

	void LimitMovement()
	{
		character.currentMovement = Mathf.Min(character.currentMovement, 1);
		if (MovementController.Instance.running && MovementController.Instance.movingCharacter == character)
		{
			MovementController.Instance.ShowMovement(character);
		}
	}

	public override string GetStatusEffectKey()
	{
		return "Sapped";
	}

	public static List<CardInstruction> GetCardInstructions()
	{
		DisplayGrid.Instance.Clear(11, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction(ExplanationItemRepository.Instance.GetExplanation("Sapped").explanationContent));

		return instructions;
	}
}
