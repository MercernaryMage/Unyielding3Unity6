using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;
using static TemplateLibrary;

public abstract class Trigger
{
	public Character owningCharacter;
    public abstract void Click();
	public abstract string GetTitle();
}

public class AttackOfOpportunityTrigger : Trigger
{
	public Character attacker;
	public Character defender;
	public Item item;
	public ActionController.AttackProfile attackProfile;

	public override void Click()
	{
		Tile attackerTile = TileGrid.Instance.FindCharacter(attacker)[0];
		Tile defenderTile = TileGrid.Instance.GetClosestCharacterTile(attackerTile, defender);
		attacker.SetFacing(TileGrid.Instance.GetFacingDirection(attackerTile, defenderTile));
		ActionController.Instance.PlayAttackAnimation(attacker, null, () =>
		{
			attackProfile.trigger = true;
			ActionController.Instance.AttackCharacter(defender, attacker, attackProfile);
			TriggerDisplay.Instance.RemoveTrigger(this);
			--attacker.triggerCount;
		});
	}

	public override string GetTitle()
	{
		return $"{attacker.displayName}: {item.itemDefinition.displayName}";
	}
}

public class AttackFairyTrigger : Trigger
{
	public Character attacker;
	public Character defender;
	public Guest guest;

	public override void Click()
	{
		Direction direction = TileGrid.Instance.GetFacingDirection(guest.currentLocation, TileGrid.Instance.GetClosestCharacterTile(guest.currentLocation, defender));
		guest.gameObject.transform.localRotation = Util.GetFacing(direction);
		ActionController.Instance.PlayAttackAnimation(guest.gameObject, null, () =>
		{
			ActionController.AttackProfile attackProfile = new ActionController.AttackProfile(0,0,3);
			attackProfile.trigger = true;

			ActionController.AttackResults attackResults = new ActionController.AttackResults();

			ActionController.Instance.DamageCharacter(defender, attacker, attackProfile, attackResults);
			TriggerDisplay.Instance.RemoveTrigger(this);
			--attacker.triggerCount;
		});
	}

	public override string GetTitle()
	{
		return $"{attacker.displayName}: Attack Fairy";
	}
}