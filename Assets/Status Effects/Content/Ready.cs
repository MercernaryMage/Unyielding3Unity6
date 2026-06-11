using System.Collections.Generic;
using UnityEngine;

public class Ready : StatusEffect
{
	public override void OnCharacterFinishedMoving(CharacterFinishedMovingMessage message)
	{
		if (message.movingCharacter.hero == character.hero)
		{
			return;
		}
		if (character.triggerCount == 0)
		{
			return;
		}
		if (!TileGrid.Instance.CharactersAreAdjacent(character, message.movingCharacter))
		{
			return;
		}
		ActionController.AttackProfile profile = new ActionController.AttackProfile(1, 6, 0);
		profile.trigger = true;
		message.waitingObjects.Add(character);
		ActionController.Instance.PlayAttackAnimation(character, null, () =>
		{
			--character.triggerCount;
			character.SetFacing(TileGrid.Instance.GetFacingDirection(character, message.movingCharacter));
			ActionController.Instance.AttackCharacter(message.movingCharacter, character, profile);
			message.RemoveWaitingObject(character);
		});
		Destroy(this);
	}

	public override string GetDisplayName()
	{
		return "Ready";
	}

	public override string GetEffectText()
	{
		return "When an enemy finishes moving adjacent to this character, make an attack of opportunity.";
	}

	public static List<CardInstruction> GetCardInstructions()
	{
		DisplayGrid.Instance.Clear(11, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction($"When an enemy finishes moving adjacent to this character, make an attack of opportunity."));
		instructions.Add(new CardInstruction($"Play this characters next card"));

		return instructions;
	}
}
