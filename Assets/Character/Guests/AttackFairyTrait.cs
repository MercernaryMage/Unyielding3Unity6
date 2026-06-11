using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackFairyTrait : Trait, GuestSetter
{
	Guest guest;
	public override void AttackComplete(AttackCompleteMessage message)
	{
		if (character.hero != message.defender.hero && character.triggerCount > 0)
		{
			if (TileGrid.Instance.GetDistanceBetweenCharacterAndTile(message.defender, guest.currentLocation) <= 3)
			{
				AttackFairyTrigger attackFairyTrigger = new AttackFairyTrigger();
				attackFairyTrigger.owningCharacter = character;
				attackFairyTrigger.attacker = character;
				attackFairyTrigger.defender = message.defender;
				attackFairyTrigger.guest = guest;
				message.raisedTriggers.Add(attackFairyTrigger);
			}
		}
	}

	public void SetGuest(Guest g)
	{
		guest = g;
	}
}
