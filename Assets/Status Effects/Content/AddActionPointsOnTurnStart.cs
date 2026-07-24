using UnityEngine;

public class AddActionPointsOnTurnStart : StatusEffect
{
	public int amount;

	public override void CharacterStartTurn(CharacterStartTurnMessage characterStartTurnMessage)
	{
		if (characterStartTurnMessage.character != character)
		{
			return;
		}
		character.actionCount += amount;
		FloatingCombatNumberController.Instance.QueueFloatingCombatNumber(character, $"+{amount} actions");
		Destroy(this);
	}

	public override string GetExplanationName()
	{
		return "Energized";
	}

}
