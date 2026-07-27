using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Recover : ReactionBase
{
	public override void Execute()
	{
		AnimationController.Instance.ShowTiles(null, Tile.OverlayType.PossibleMovement, Delay);
	}

	public void Delay()
	{
		ActionController.Instance.PlayAdvancedAttackAnimation(owningCharacter, null, cardScriptableObject.effects[0], null, () =>
		{
			Injured injured = owningCharacter.GetComponent<Injured>();
			if (injured != null)
			{
				GameObject.Destroy(injured);
			}

			owningCharacter.armor = Mathf.Min(owningCharacter.armor + GetIntValue("Value"), owningCharacter.characterDefinition.armor);
			owningCharacter.maxArmor = Mathf.Max(owningCharacter.maxArmor, owningCharacter.armor);
			BattleController.ReturnControlToPlayer();
		});
	}

	public static List<CardInstruction> GetCardInstructions(CardScriptableObject scriptableObject)
	{
		DisplayGrid.Instance.Clear(12, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction("Remove <u>Injured</u>"));
		instructions.Add(new CardInstruction($"Restore up to {scriptableObject.GetTagIntValue("Value")} armor"));
		return instructions;
	}
}
