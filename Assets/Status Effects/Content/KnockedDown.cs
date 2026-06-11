using UnityEngine;

public class KnockedDown : StatusEffect
{
	public override string GetDisplayName()
	{
		return "Knocked Down";
	}

	public override void CharacterStartTurn(CharacterStartTurnMessage characterStartTurnMessage)
	{
		if (characterStartTurnMessage.character != character)
		{
			return;
		}

		Destroy(this);
		if (character.hero == true)
		{
			character.currentMovement = 0;
		}
	}

	//Evasion is capped at 5?
	//No can't gain reaction points???

	public override string GetEffectText()
	{
		return "";
	}
}
