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
		instructions.Add(new CardInstruction("The marked area moves with this character until the blast goes off."));

		return instructions;
	}
}
