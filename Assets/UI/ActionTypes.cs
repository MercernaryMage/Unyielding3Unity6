using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.Linq;
using UnityEngine.Profiling;
using static UnityEngine.GraphicsBuffer;
using static ActionPattern;

public class ActionTypes : MonoBehaviour
{
	static public void DoPostAction(Character c1, Character c2, ActionPattern pattern, Item item, string functionName)
	{
		System.Object[] paramsList = new System.Object[]
		{
			c1,
			c2,
			pattern,
			item
		};
		Type.GetType("ActionTypes").GetMethod(functionName).Invoke(null, paramsList);
	}

	static public void DoTargetedAction(Character c1, Character c2, ActionPattern pattern)
	{
		System.Object[] paramsList = new System.Object[]
		{
			c1,
			c2,
			pattern,
		};
		Type.GetType("ActionTypes").GetMethod(pattern.targetedAction.actionName).Invoke(null, paramsList);
	}

	static public void DoGeoTargetedAction(Character c, Tile t, ActionPattern pattern)
	{
		System.Object[] paramsList = new System.Object[]
		{
			c,
			t,
			pattern,
		};
		Type.GetType("ActionTypes").GetMethod(pattern.targetedAction.actionName).Invoke(null, paramsList);
	}

	static public Tuple<bool, string> DoDisqualifierForAction(Character c1, Character c2, ActionPattern pattern)
	{
		if (string.IsNullOrEmpty(pattern.targetedAction.disqualiferFunction))
		{
			return new Tuple<bool, string>(true, "");
		}

		System.Object[] paramsList = new System.Object[]
		{
			c1,
			c2,
			pattern,
		};
		System.Object returnObject = Type.GetType("ActionTypes").GetMethod(pattern.targetedAction.disqualiferFunction).Invoke(null, paramsList);
		return (Tuple<bool, string>)returnObject;
	}

	static public Tuple<bool, string> DoDisqualifierForInstantAction(Character c, ActionPattern pattern)
	{
		if (string.IsNullOrEmpty(pattern.instantAction.disqualiferFunction))
		{
			return new Tuple<bool, string>(true, "");
		}

		System.Object[] paramsList = new System.Object[]
		{
			c,
			pattern
		};
		System.Object returnObject = Type.GetType("ActionTypes").GetMethod(pattern.instantAction.disqualiferFunction).Invoke(null, paramsList);
		return (Tuple<bool, string>)returnObject;
	}

	static public bool DoDisqualifierForActionButton(Character c1, ActionPattern pattern, Item item)
	{
		System.Object[] paramsList = new System.Object[]
		{
			c1,
			pattern,
			item
		};
		System.Object returnObject = Type.GetType("ActionTypes").GetMethod(pattern.disqualifierFunc).Invoke(null, paramsList);
		return (bool)returnObject;
	}

	static public string GetDescription(Character c, ActionPattern pattern)
	{
		System.Object[] paramsList = new System.Object[]
		{
			c,
			pattern
		};
		System.Object returnObject = Type.GetType("ActionTypes").GetMethod(pattern.actionDescriptionFunction).Invoke(null, paramsList);
		return (string)returnObject;
	}

	static public void DoInstantAction(Character c, ActionPattern pattern, bool alternate = false)
	{
		System.Object[] paramsList = new System.Object[]
		{
			c,
			pattern,
			alternate
		};
		Type.GetType("ActionTypes").GetMethod(pattern.instantAction.actionName).Invoke(null, paramsList);
	}

	static public void PayActionPatternCost(Character c, ActionPattern pattern, Item item = null)
	{
		c.currentMovement -= pattern.cost.movement;
		c.actionCount -= pattern.cost.actions;
		c.SpendEnergy(pattern.cost.energy);
	}

	static public void PayActionPatternCharges(ActionPattern pattern, Item item)
	{
		item.charges -= pattern.chargesTaken;
		if (pattern.keywords.ContainsIgnoreCase("Loading"))
		{
			item.Unload();
		}
	}

	/////////TARGET SOMEONE ELSE////////////
	static public void Scry(Character c1, Character c2, ActionPattern pattern)
	{
		
		AICardDisplay.Instance.ShowCard(c2.cards[0].cardScriptableObject);
		c2.cards[0].isRevealed = true;
		ActionController.Instance.EndAction();
	}

	static public void Behold(Character c1, Character c2, ActionPattern pattern)
	{
		CardSwapController.Instance.Set(c2.reactions[0], c2.reactions[1], c2);
	}

	static public void Heal(Character c1, Character c2, ActionPattern pattern)
	{
		int health = 0;
		List<(int number, int face, int flat, ActionPattern.DamageType type)> damages = Util.ParseDiceString(pattern.damageString);
		for (int i = 0; i < damages[0].number; ++i)
		{
			health += Util.RollDice(1, damages[0].face);
		}
		health += damages[0].flat;
		FloatingCombatNumberController.Instance.QueueFloatingCombatNumber(c2, health.ToString());
		c2.currentHP = Math.Min(c2.currentHP + c2.maxHP / 2, c2.maxHP);
		ActionController.Instance.EndAction();
	}

	static public void HalfHeal(Character c1, Character c2, ActionPattern pattern)
	{
		int i = c2.maxHP / 2;
		c2.currentHP = Math.Min(c2.currentHP + c2.maxHP / 2, c2.maxHP);
		ActionController.Instance.EndAction();
	}

	static public void GiveStatusEffect(Character c1, Character c2, ActionPattern pattern)
	{
		c2.AddStatusEffect(System.Type.GetType(pattern.stringParam), null);
		ActionController.Instance.EndAction();
	}

	static public void SpecialPatternAttack(Character c1, Character c2, ActionPattern pattern)
	{
		ActionController.Instance.PlayAttackAnimation(c1, null, () =>
		{
			ActionController.Instance.AttackCharacter(c2, c1, Util.GetAttackProfile(pattern));
			ActionController.Instance.HandleEndOfAction();
		});
	}

	static public void BlockOther(Character c1, Character c2,  ActionPattern pattern)
	{
		c2.armor = Mathf.Max(c2.armor, pattern.intParam);
		c2.maxArmor = Mathf.Max(c2.maxArmor, pattern.intParam);
		FloatingCombatNumberController.Instance.QueueFloatingCombatNumber(c2, $"+{pattern.intParam} armor");
		ActionController.Instance.EndAction();
	}
	
	static public void ExecuteInstantActionOnTarget(Character c1, Character c2, ActionPattern pattern)
	{
		ActionController.Instance.OverrideCurrentAction(pattern.storedPattern);
		if (c1 != c2)
		{
			DoInstantAction(c2, pattern.storedPattern, true);
		}
		else
		{
			DoInstantAction(c2, pattern.storedPattern, false);
		}
		ActionController.Instance.EndAction();
	}

	static public void LassoSelect(Character c1, Character c2, ActionPattern pattern)
	{
		ActionPattern targetingPattern = new ActionPattern();
		targetingPattern.useTargetedAction = true;
		targetingPattern.targetedAction = new TargetedAction();
		targetingPattern.targetedAction.actionName = "PullCharacterFromLasso";
		targetingPattern.targetedAction.targetType = TargetType.Directions;
		targetingPattern.aoeType = AoEType.Directions;
		targetingPattern.forcedTarget = c2;
		targetingPattern.cost = pattern.cost;
		targetingPattern.keywords = pattern.keywords;
		targetingPattern.postActionCommands = pattern.postActionCommands;

		ActionController.Instance.ShowAttackableTiles(c1, ActionController.Instance.currentItem, targetingPattern);
	}

	static public void PullCharacterFromLasso(Character c, Tile t, ActionPattern pattern)
	{
		ActionController.Instance.KnockBack(pattern.forcedTarget, new List<Tile>() {t}, -5);

		ActionController.Instance.attackingCharacter = c;
		ActionController.Instance.EndAction();
	}

	/////////Target Other + item///////////////
	static public void Backstab(Character attacker, Character defender, ActionPattern pattern, Item i)
	{
		if (!defender.alive)
		{
			return;
		}

		if (!TileGrid.Instance.IsFacing(defender, attacker))
		{
			defender.AddStatusEffect(typeof(WeakPoint), null);
		}
	}

	/////////Target Other Disqualifiers////////

	static public Tuple<bool, string> AllowedToHealTarget(Character c1, Character c2, ActionPattern pattern)
	{
		return new Tuple<bool, string>(c2.currentHP < c2.maxHP, $"{c2.displayName} is at full health");
	}

	static public Tuple<bool, string> StatusEffectStacking(Character c1, Character c2, ActionPattern pattern)
	{
		Component c = c2.gameObject.GetComponent(System.Type.GetType(pattern.stringParam));
		return new Tuple<bool, string>(c == null, $"{c2.displayName} already has this effect");
	}

	static public Tuple<bool, string> IsBehindEnemy(Character c1, Character c2, ActionPattern pattern)
	{
		return new Tuple<bool, string>(!TileGrid.Instance.IsFacing(c2, c1), $"{c1.displayName} is not behind {c2.displayName}");
	}

	static public Tuple<bool, string> CanBlockOther(Character c, Character c2, ActionPattern pattern)
	{
		if (c2.armor >= pattern.intParam)
		{
			return new Tuple<bool, string>(false, $"{c2.displayName} has armor");
		}
		return new Tuple<bool, string>(true, "");
	}

	static public Tuple<bool, string> DiqaulifyInstantActionOnTarget(Character c, Character c2, ActionPattern pattern)
	{
		return DoDisqualifierForInstantAction(c2, pattern.storedPattern);
	}

	static public Tuple<bool, string> IsScanableReactions(Character c1, Character c2, ActionPattern pattern)
	{
		if (c2.reactions.Count < 2)
		{
			return new Tuple<bool, string>(false, $"{c2.displayName} is not scannable");
		}
		return new Tuple<bool, string>(c2.characterDefinition.isScannable, $"{c2.displayName} is not scannable");
	}

	static public Tuple<bool, string> IsScanableCards(Character c1, Character c2, ActionPattern pattern)
	{
		if (c2.cards.Count < 2)
		{
			return new Tuple<bool, string>(false, $"{c2.displayName} is not scannable");
		}
		return new Tuple<bool, string>(c2.characterDefinition.isScannable, $"{c2.displayName} is not scannable");
	}

	/////////SELF TARGET////////////

	static public void Ready(Character c, ActionPattern pattern, bool alternate)
	{
		foreach (Item item in c.storageCharacter.equipment)
		{
			if (!item.loaded)
			{
				item.Load();
			}
		}
		c.RestoreAllEnergy();
		PayActionPatternCost(c, pattern);
	}

	static public void Surge(Character c, ActionPattern pattern, bool alternate)
	{
		BattleController.Instance.HandleSurge(c);
		PayActionPatternCost(c, pattern);
	}

	static public void Dash(Character c, ActionPattern pattern, bool alternate)
	{
		BattleController.Instance.HandleDash(c);
		PayActionPatternCost(c, pattern);
	}

	static public void FullHeal(Character c, ActionPattern pattern, bool alternate)
	{
		c.currentHP = c.maxHP;
		FloatingCombatNumberController.Instance.QueueFloatingCombatNumber(c, "Full Heal!");
		PayActionPatternCost(c, pattern);
	}

	static public void GiveActionPoints(Character c, ActionPattern pattern, bool alternate)
	{
		if (alternate)
		{
			AddActionPointsOnTurnStart effect = (AddActionPointsOnTurnStart)c.AddStatusEffect(typeof(AddActionPointsOnTurnStart), null);
			effect.amount = pattern.intParam;
		}
		else
		{
			c.actionCount += pattern.intParam;
			FloatingCombatNumberController.Instance.QueueFloatingCombatNumber(c, $"+{pattern.intParam} actions");
			PayActionPatternCost(c, pattern);
		}
		
	}


	static public void Block(Character c, ActionPattern pattern, bool alternate)
	{
		c.armor = Mathf.Max(c.armor, pattern.intParam);
		c.maxArmor = Mathf.Max(c.maxArmor, pattern.intParam);
		PayActionPatternCost(c, pattern);
		FloatingCombatNumberController.Instance.QueueFloatingCombatNumber(c, $"+{pattern.intParam} armor");
	}

	static public void SpawnGuest(Character c, Tile t, ActionPattern pattern)
	{
		GameObject prefab = Resources.Load<GameObject>($"Guests/{pattern.stringParam}");
		GameObject guestObject = GameObject.Instantiate(prefab, t.transform.parent);
		guestObject.transform.localPosition = new Vector3(0, 0, 0);
		Guest guest = guestObject.GetComponent<Guest>();
		guest.currentLocation = t;
		guest.owner = c;
		t.guest = guest;
		if (guest.traitName != "")
		{
			Component comp = c.gameObject.AddComponent(Type.GetType(guest.traitName));
			guest.createdComponent = comp;
			if (comp is GuestSetter setter)
			{
				setter.SetGuest(guest);
			}
		}
		PayActionPatternCost(c, pattern);
		ActionController.Instance.EndAction();
	}
	//self target disqualifiers

	static public Tuple<bool, string> CanBlock(Character c, ActionPattern pattern)
	{
		if (c.armor >= pattern.intParam)
		{
			return new Tuple<bool, string>(false, $"{c.displayName} has armor");
		}
		return new Tuple<bool, string>(true, "");
	}

	static public Tuple<bool, string> IsFullHP(Character c, ActionPattern pattern)
	{
		if (c.currentHP >= c.maxHP)
		{
			return new Tuple<bool, string>(false, $"{c.displayName} is full hp");
		}
		return new Tuple<bool, string>(true, "");
	}

	static public Tuple<bool, string> CanReady(Character c, ActionPattern pattern)
	{
		bool hasWeaponToReload = c.storageCharacter.equipment.Any(item => !item.loaded);
		bool isMaxEnergy = c.currentEnergy >= c.characterDefinition.maxEnergy;
		if (!hasWeaponToReload && isMaxEnergy)
		{
			return new Tuple<bool, string>(false, "Nothing to ready");
		}
		return new Tuple<bool, string>(true, "");
	}

	///////////descriptions///////////
	static public string GetSurgeDescription(Character c, ActionPattern pattern)
	{
		if (c.storageCharacter.surgeIndex == 0)
		{
			return "Gain 2 ap at the cost of 1 energy";
		}
		else if (c.storageCharacter.surgeIndex == 1)
		{
			return "Gain 2 ap at the cost of 1d3 energy";
		}
		else
		{
			int addedCost = Mathf.Min(c.storageCharacter.surgeIndex - 2, 4);
			return "Gain 2 ap at the cost of 1d6+" + addedCost + " energy";
		}
	}

}
