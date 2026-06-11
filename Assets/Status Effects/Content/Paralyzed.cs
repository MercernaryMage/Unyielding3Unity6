using UnityEngine;

public class Paralyzed : StatusEffect
{
	public override void CharacterStartTurn(CharacterStartTurnMessage characterStartTurnMessage)
	{
		if (characterStartTurnMessage.character != character)
		{
			return;
		}
		character.currentMovement = 0;
	}

	public override void CharacterEndTurn(CharacterEndTurnMessage characterEndTurnMessage)
	{
		if (characterEndTurnMessage.character != character)
		{
			return;
		}
		Destroy(this);
	}

	public override string GetDisplayName()
	{
		return "Paralyzed";
	}

	public override string GetEffectText()
	{
		return "Loses all movement at the start of their turn. Removed at the end of their turn.";
	}
}
