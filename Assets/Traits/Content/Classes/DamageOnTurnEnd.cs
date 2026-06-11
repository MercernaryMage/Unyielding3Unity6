using System.Collections.Generic;
using UnityEngine;

public class DamageOnTurnEnd : Trait
{
	public override void CharacterEndTurn(CharacterEndTurnMessage message)
	{
		if (message.character.hero != character.hero)
		{
			if (TileGrid.AreCharactersAdjacent(message.character, character))
			{
				ActionController.AttackResults results = new ActionController.AttackResults();
				results.fakeHit = true;
				ActionController.Instance.DamageCharacter(message.character, character, new ActionController.AttackProfile(0, 0, 4), results);
			}
		}
	}
}
