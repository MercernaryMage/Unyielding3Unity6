using System.Collections.Generic;
using UnityEngine;

public class Stalked : TileTrackingEffect
{
	public override string GetExplanationName()
	{
		return "Stalked";
	}

	public static List<CardInstruction> GetCardInstructions()
	{
		DisplayGrid.Instance.Clear(11, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction("The marked tile moves with this character until the stalker attacks it."));

		return instructions;
	}
}
