using UnityEngine;

public class Vulnerable : StatusEffect
{
	public override string GetDisplayName()
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

	public override string GetEffectText()
	{
		return "This character takes double damage.";
	}
}
