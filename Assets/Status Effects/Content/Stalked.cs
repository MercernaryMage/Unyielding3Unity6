using System.Collections.Generic;
using UnityEngine;

public class Stalked : TileTrackingEffect
{
	public override string GetStatusEffectKey()
	{
		return "Stalked";
	}

	public static List<CardInstruction> GetCardInstructions()
	{
		DisplayGrid.Instance.Clear(11, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction(ExplanationItemRepository.Instance.GetExplanation("Stalked").explanationContent));

		return instructions;
	}
}
