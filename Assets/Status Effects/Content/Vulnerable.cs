using UnityEngine;

public class Vulnerable : StatusEffect
{
	public override string GetStatusEffectKey()
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

	public override string GetIconName()
	{
		return "status_symbol_vulnerable";
	}
}
