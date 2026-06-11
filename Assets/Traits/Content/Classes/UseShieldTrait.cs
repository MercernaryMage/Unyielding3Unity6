using System.Collections.Generic;
using UnityEngine;

public class UseShieldTrait : Trait
{
	bool movementUsed = false;

	public override void CharacterStartTurn(CharacterStartTurnMessage message)
	{
		if (message.character == character)
		{
			movementUsed = false;
		}
	}

	public override void CharacterStartMoving(CharacterStartMovementMessage message)
	{
		if (message.movingCharacter == character)
		{
			movementUsed = true;
		}
	}

	public override void CharacterEndTurn(CharacterEndTurnMessage message)
	{
		if (movementUsed == true)
		{
			return;
		}

		if (message.character == character)
		{
			foreach (Item i in character.storageCharacter.equipment)
			{
				if (i.itemDefinition.name == "Shield")
				{
					ActionTypes.DoInstantAction(character, i.itemDefinition.actions[0]);
					HeroDisplayRouter.Instance.mainDisplay.UpdateWithLastCharacter();
					FloatingCombatNumberController.Instance.QueueFloatingCombatNumber(character, "using shield");
					break;
				}
			}
		}
	}
}
