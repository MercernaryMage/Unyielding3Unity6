using UnityEngine;

public class Vulnerable : StatusEffect
{
	public override string GetExplanationName()
	{
		return "Vulnerable";
	}

	public override void CharacterEndTurn(CharacterEndTurnMessage characterEndTurnMessage)
	{
		if (characterEndTurnMessage.character == character && character.currentEnergy > 0)
		{
			Destroy(this);
		}
	}

}
