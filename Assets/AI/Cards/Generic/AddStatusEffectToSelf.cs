using UnityEngine;
using System.Collections.Generic;
using System;

public class AddStatusEffectToSelf : Card
{
	public override void Execute()
	{
		AnimationController.Instance.ShowTiles(null, Tile.OverlayType.PossibleMovement, Delay, null, 3);
	}


	void Delay()
	{
		owningCharacter.AddStatusEffect(Type.GetType(GetStringValue("Param")), null);
		if (GetBoolValue("DrawExtraCard"))
		{
			AIController.Instance.Reshuffle(owningCharacter);
			AIController.Instance.TakeTurn(owningCharacter);
		}
		else
		{
			Finish();
		}
	}

	public static List<CardInstruction> GetCardInstructions(CardScriptableObject scriptableObject)
	{
		string className = scriptableObject.GetTagStringValue("Param");
		object o = Type.GetType(className).GetMethod("GetCardInstructions").Invoke(null, null);
		List<CardInstruction> instructions = (List<CardInstruction>)o;
		return instructions;
	}
}
