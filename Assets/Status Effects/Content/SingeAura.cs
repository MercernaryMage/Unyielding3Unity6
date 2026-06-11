using System.Collections.Generic;
using UnityEngine;

public class SingeAura : StatusEffect
{
	public override void OnCharacterTakeBurnDamage(CharacterTakingBurnDamageMessage message)
	{
		if (message.character.hero == character.hero)
		{
			return;
		}
		if (TileGrid.Instance.GetDistanceBetweenCharacters(character, message.character) > 1)
		{
			return;
		}
		message.autoFail = true;
	}

	public override void OnHeroDowned(HeroDownedMessage heroDownedMessage)
	{
		Destroy(this);
	}

	public override string GetDisplayName()
	{
		return "Singe Aura";
	}

	public override string GetEffectText()
	{
		return "Enemies within 1 tile automatically fail their burn damage roll.";
	}

	public static List<CardInstruction> GetCardInstructions()
	{
		DisplayGrid.Instance.Clear(11, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction($"Enemies within 1 tile automatically fail their burn damage roll.  Removed when a hero is Downed."));
		instructions.Add(new CardInstruction($"Play this characters next card"));

		return instructions;
	}
}
