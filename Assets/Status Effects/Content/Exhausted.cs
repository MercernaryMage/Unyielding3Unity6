using UnityEngine;

public class Exhausted : StatusEffect
{
	public override string GetDisplayName()
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

	public override string GetEffectText()
	{
		return "This character gains half action points and half movement points at the start of their turn.";
	}
}
