using UnityEngine;

public class Exhausted : StatusEffect
{
	public override string GetExplanationName()
	{
		return "Exhausted";
	}

	public override void CharacterEndTurn(CharacterEndTurnMessage characterEndTurnMessage)
	{
		if (characterEndTurnMessage.character == character && character.currentEnergy > 0)
		{
			Destroy(this);
		}
	}

}
