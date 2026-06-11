using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Downed : StatusEffect
{
	public int overflow;

	public override void CharacterEndTurn(CharacterEndTurnMessage characterEndTurnMessage)
	{
		if (characterEndTurnMessage.character == character)
		{
			ClearEffect();
			Destroy(this);
		}
	}

	public void ClearEffect()
	{
		while (overflow > character.maxHP)
		{
			--character.storageCharacter.currentDetermination;
			overflow -= character.maxHP;
		}

		if (character.storageCharacter.currentDetermination <= 0)
		{
			ActionController.Instance.KillCharacter(character);
		}
		else
		{
			character.currentHP = character.maxHP - overflow;
			--character.storageCharacter.currentDetermination;
			overflow = 0;
		}
	}

	public override string GetDisplayName()
	{
		return "Downed";
	}

	public override string GetEffectText()
	{
		return "Cannot take actions or move.  Will be removed at the start of the next turn.";
	}
}
