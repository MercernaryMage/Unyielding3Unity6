using System.Collections.Generic;
using UnityEngine;

public class Following : TileTrackingEffect
{
	public override string GetStatusEffectKey()
	{
		return "Following";
	}

	public static List<CardInstruction> GetCardInstructions()
	{
		DisplayGrid.Instance.Clear(11, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction(ExplanationItemRepository.Instance.GetExplanation("Following").explanationContent));

		return instructions;
	}
}
