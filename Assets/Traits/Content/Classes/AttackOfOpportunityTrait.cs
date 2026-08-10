using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackOfOpportunityTrait : Trait
{
	public override void OnPreviewMovementProvoke(PreviewMovementProvokeMessage message)
	{
		if (WouldProvoke(message.movingCharacter))
		{
			message.provoked = true;
		}
	}

	bool WouldProvoke(Character mover)
	{
		if (mover.GetComponent<Disengage>() != null)
		{
			return false;
		}
		if (mover.IsDowned())
		{
			return false;
		}
		if (character.triggerCount <= 0)
		{
			return false;
		}
		if (mover.hero == character.hero)
		{
			return false;
		}
		if (!character.alive || !mover.alive)
		{
			return false;
		}
		if (character.hero == false)
		{
			return TileGrid.Instance.CharactersAreAdjacent(character, mover);
		}
		int distance = TileGrid.Instance.GetDistanceBetweenCharacters(character, mover);
		foreach (Item item in character.storageCharacter.equipment)
		{
			if (!item.itemDefinition.weapon)
			{
				continue;
			}
			if (item.itemDefinition.actions[0].keywords.ContainsIgnoreCase("Loading") && item.loaded == false)
			{
				return false;
			}
			for (int i = 0; i < item.itemDefinition.actions.Count; ++i)
			{
				
				if (distance > item.itemDefinition.actions[i].threatRange)
				{
					return false;
				}
			}
			return true;
		}
		return false;
	}

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
		if (message.movingCharacter.IsDowned())
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
							if (item.itemDefinition.actions[i].keywords.ContainsIgnoreCase("Loading") && item.loaded == false)
							{
								continue;
							}

							AttackOfOpportunityTrigger attackOfOpportunityTrigger = new AttackOfOpportunityTrigger();
							attackOfOpportunityTrigger.owningCharacter = character;
							attackOfOpportunityTrigger.attacker = character;
							attackOfOpportunityTrigger.defender = message.movingCharacter;
							attackOfOpportunityTrigger.item = item;
							attackOfOpportunityTrigger.actionPattern = item.itemDefinition.actions[i];
							attackOfOpportunityTrigger.attackProfile = Util.GetAttackProfile(item.itemDefinition.actions[i]);
							message.raisedTriggers.Add(attackOfOpportunityTrigger);
						}
					}
				}
			}
		}
	}
}
