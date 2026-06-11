using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackOfOpportunityTrait : Trait
{
	public override void CharacterStartMoving(CharacterStartMovementMessage message)
	{
		if (!message.provokeTriggers)
		{
			return;
		}
		if (message.movingCharacter.GetComponent<Disengage>() != null)
		{
			return;
		}
		if (character.triggerCount <= 0)
		{
			return;
		}
		if (message.movingCharacter.hero != character.hero)
		{
			if (character.hero == false)
			{
				if (TileGrid.Instance.CharactersAreAdjacent(character, message.movingCharacter))
				{
					AttackOfOpportunityTrigger attackOfOpportunityTrigger = new AttackOfOpportunityTrigger();
					attackOfOpportunityTrigger.owningCharacter = character;
					attackOfOpportunityTrigger.attacker = character;
					attackOfOpportunityTrigger.defender = message.movingCharacter;
					attackOfOpportunityTrigger.attackProfile = new ActionController.AttackProfile(1, 6, 0);
					message.raisedTriggers.Add(attackOfOpportunityTrigger);
				}
			}
			else
			{
				int distance = TileGrid.Instance.GetDistanceBetweenCharacters(character, message.movingCharacter);
				foreach (Item item in character.storageCharacter.equipment)
				{
                    if ( !item.itemDefinition.weapon)
                    {
						continue;
                    }
                    for (int i = 0; i < item.itemDefinition.actions.Count; ++i)
					{
						if (distance <= item.itemDefinition.actions[i].threatRange)
						{
							AttackOfOpportunityTrigger attackOfOpportunityTrigger = new AttackOfOpportunityTrigger();
							attackOfOpportunityTrigger.owningCharacter = character;
							attackOfOpportunityTrigger.attacker = character;
							attackOfOpportunityTrigger.defender = message.movingCharacter;
							attackOfOpportunityTrigger.item = item;
							attackOfOpportunityTrigger.attackProfile = Util.GetAttackProfile(item.itemDefinition.actions[i]);
							message.raisedTriggers.Add(attackOfOpportunityTrigger);
						}
					}
				}
			}
		}
	}
}
