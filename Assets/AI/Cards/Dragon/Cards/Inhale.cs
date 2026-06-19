using System;
using System.Collections.Generic;
using UnityEngine;

public class Inhale : Card
{
	public override void Execute()
	{
		AnimationController.Instance.ShowTiles(null, Tile.OverlayType.PossibleMovement, Delay, null, 3);
	}

	public void Delay()
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

		AIController.Instance.Reshuffle(owningCharacter);
		AIController.Instance.TakeTurn(owningCharacter);
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
