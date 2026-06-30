using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Screech : ReactionBase
{
	public override void Execute()
	{
		AnimationController.Instance.ShowTiles(null, Tile.OverlayType.PossibleMovement, Delay);
	}

	public void Delay()
	{
		AnimationController.PlayEffect(owningCharacter, cardScriptableObject.effects[0]
			, .25f, () =>
			{
				HowlAndGrowl.Growl(owningCharacter, GetIntValue("Value"));
				BattleController.ReturnControlToPlayer();
			});
	}

	public static List<CardInstruction> GetCardInstructions(CardScriptableObject scriptableObject)
	{
		DisplayGrid.Instance.Clear(12, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction($"Growl, aggravating {scriptableObject.GetTagIntValue("Value")} reactions"));
		return instructions;
	}
}
