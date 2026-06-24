using System.Collections.Generic;
using UnityEngine;

public class Fiery : Trait
{
	public override void CombatStart(CombatStartMessage message)
	{
		for (int i = 0; i < character.cards.Count; ++i)
		{
			if (character.cards[i].cardScriptableObject.name == "Fire Breath")
			{
				Card c = character.cards[i];
				character.cards.RemoveAt(i);
				character.cards.Add(c);
				return;
			}
		}
		Debug.LogError("Fire Breath not found on a character with Fiery");
	}

	public override void CharacterAttack(CharacterAttackingMessage message)
	{
		if (message.defender != character)
		{
			return;
		}

		bool areAdjacent = TileGrid.Instance.CharactersAreAdjacent(message.attacker, character);
		bool ranged = message.pattern.keywords.Contains("Ranged");
		if (areAdjacent)
		{
			ActionController.AddBurning(message.attacker, 2);
		}
		if (areAdjacent && !ranged && message.backstab)
		{
			for (int i = 0; i < character.cards.Count; ++i)
			{
				if (character.cards[i].cardScriptableObject.name == "Fire Breath")
				{
					if (i == character.cards.Count - 1)
					{
						return;
					}
					Card c = character.cards[i];
					character.cards.RemoveAt(i);
					int newIndex = Mathf.Min(i + 5, character.cards.Count);
					character.cards.Insert(newIndex, c);
					return;
				}
			}
		}
		else
		{
			for (int i = 0; i < character.cards.Count; ++i)
			{
				if (character.cards[i].cardScriptableObject.name == "Fire Breath")
				{
					if (i == 0)
					{
						return;
					}
					Card c = character.cards[i];
					character.cards.RemoveAt(i);
					character.cards.Insert(i - 1, c);
					return;
				}
			}
		}
	}

	public override void OnCharacterTakeBurnDamage(CharacterTakingBurnDamageMessage message)
	{
		if (message.character.hero == character.hero)
		{
			return;
		}
		int dist = TileGrid.Instance.GetDistanceBetweenCharacters(character, message.character);
		if (dist == 1)
		{
			return;
		}
		message.autoPass = true;
	}



}
