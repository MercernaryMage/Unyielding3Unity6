using System;
using System.Collections.Generic;
using UnityEngine;

public class Inhale : Card
{
	public override void Execute()
	{
		List<Card> deck = owningCharacter.cards;
		int fireBreathIndex = -1;
		for (int i = 1; i < deck.Count; i++)
		{
			if (deck[i].cardScriptableObject.className == "FireBreath")
			{
				fireBreathIndex = i;
				break;
			}
		}

		if (fireBreathIndex > 0)
		{
			Card fireBreath = deck[fireBreathIndex];
			deck.RemoveAt(fireBreathIndex);
			deck.Insert(Math.Max(1, fireBreathIndex - 4), fireBreath);
		}

		Finish();
	}

	public static List<CardInstruction> GetCardInstructions(CardScriptableObject scriptableObject)
	{
		DisplayGrid.Instance.Clear(11, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction("Find Fire Breath in the deck"));
		instructions.Add(new CardInstruction("and move it up 4 spaces"));
		DisplayGrid.Instance.Show();
		return instructions;
	}
}
