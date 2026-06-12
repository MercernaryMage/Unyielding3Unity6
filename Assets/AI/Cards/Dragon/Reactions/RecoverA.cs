using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RecoverA : ReactionBase
{
	public override void Execute()
	{
		AnimationController.Instance.ShowTiles(null, Tile.OverlayType.PossibleMovement, Delay);
	}

	public void Delay()
	{
		Injured injured = owningCharacter.GetComponent<Injured>();
		if (injured != null)
		{
			GameObject.Destroy(injured);
		}

		owningCharacter.armor = Mathf.Min(owningCharacter.armor + GetIntValue("Value"), owningCharacter.characterDefinition.armor);

		BattleController.ReturnControlToPlayer();
	}

	public static List<CardInstruction> GetCardInstructions(CardScriptableObject scriptableObject)
	{
		DisplayGrid.Instance.Clear(12, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction("Remove Injured"));
		instructions.Add(new CardInstruction($"Restore up to {scriptableObject.GetTagIntValue("Value")} armor"));
		return instructions;
	}
}
