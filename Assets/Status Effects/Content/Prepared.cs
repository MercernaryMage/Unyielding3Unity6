using System.Collections.Generic;
using UnityEngine;

public class Prepared : StatusEffect
{
	public override void OnCharacterFinishedMoving(CharacterFinishedMovingMessage message)
	{
		if (message.movingCharacter.hero == character.hero)
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
			character.SetFacing(TileGrid.Instance.GetFacingDirection(character, message.movingCharacter));
			ActionController.Instance.AttackCharacter(message.movingCharacter, character, profile);
			message.RemoveWaitingObject(character);
		});
	}

	public override void OnHeroDowned(HeroDownedMessage heroDownedMessage)
	{
		Destroy(this);
	}

	public override string GetStatusEffectKey()
	{
		return "Readied";
	}


	public static List<CardInstruction> GetCardInstructions()
	{
		DisplayGrid.Instance.Clear(11, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction(ExplanationItemRepository.Instance.GetExplanation("Readied").explanationContent));
		return instructions;
	}
}
