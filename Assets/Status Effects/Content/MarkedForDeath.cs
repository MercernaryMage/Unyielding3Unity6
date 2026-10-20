using System.Collections.Generic;
using UnityEngine;

public class MarkedForDeath : StatusEffect
{
	public Character causingCharacter;
	public int value = 5;

	bool sawCausingCharacterTurnStart;

	public override void CharacterStartTurn(CharacterStartTurnMessage characterStartTurnMessage)
	{
		if (characterStartTurnMessage.character != causingCharacter)
		{
			return;
		}
		sawCausingCharacterTurnStart = true;
	}

	public override void CharacterEndTurn(CharacterEndTurnMessage characterEndTurnMessage)
	{
		if (characterEndTurnMessage.character != causingCharacter)
		{
			return;
		}
		if (!sawCausingCharacterTurnStart)
		{
			return;
		}
		Destroy(this);
	}

	public override void OnPreDamageDealt(PreDamageDealtMessage preDamageDealtMessage)
	{
		if (preDamageDealtMessage.defender != character)
		{
			return;
		}
		if (preDamageDealtMessage.attacker != causingCharacter)
		{
			return;
		}
		if (preDamageDealtMessage.damage <= 0)
		{
			return;
		}

		preDamageDealtMessage.damage += value;
		if (preDamageDealtMessage.results != null)
		{
			preDamageDealtMessage.results.outString += $" + {value} ({GetExplanationName()})";
		}
		Destroy(this);
	}

	public override string GetStatusEffectKey()
	{
		return "MarkedForDeath";
	}

	public static List<CardInstruction> GetCardInstructions()
	{
		DisplayGrid.Instance.Clear(11, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction(ExplanationItemRepository.Instance.GetExplanation("MarkedForDeath").explanationContent));

		return instructions;
	}

	public override bool StatusEffectIsVisibleInHealthBar()
	{
		return true;
	}
}
