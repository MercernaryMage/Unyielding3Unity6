using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.UI;
using static CharacterScriptableObject;

public class SlotDisplay : MonoBehaviour
{
	[Serializable]
	public class SlotElement
	{
		public GameObject obj;
		public TextMeshProUGUI weaponName;
		public TextMeshProUGUI keywords;
		public TextMeshProUGUI stats;
		public TextMeshProUGUI rules;
		public Image itemIcon;
	}

	public TextMeshProUGUI slotTypeText;

	public SlotElement normal;

	WeaponSlotType slotType;
	WeaponSlot slot;
	Item item;

	public void Set(WeaponSlotType s, WeaponSlot slotParam, Item i)
	{
		item = i;
		slotType = s;

		string slotTypeName = ""; 

		if (s == WeaponSlotType.Support)
		{
			slotTypeName = "Support";
		}
		else if (s == WeaponSlotType.Medium)
		{
			slotTypeName = "Medium";
		}
		else if (s == WeaponSlotType.Heavy)
		{
			slotTypeName = "Heavy";
		}
		slotTypeText.text = slotTypeName;
		slot = slotParam;

		normal.obj.SetActive(true);
		if (item == null)
		{
			normal.weaponName.text = "empty";
			normal.keywords.text = "";
			normal.stats.text = "";
			normal.rules.text = "";
		}
		else
		{
			normal.weaponName.text = item.itemDefinition.displayName;
			normal.keywords.text = $"{slotTypeName}, {GetActionKeywords(item.itemDefinition, item.itemDefinition.actions[0])}";
			//"Range 5, 1d6 + 2";
			//"Range 20, Blast 1, 2d6"
			//range, AOE, damage
			if (item.itemDefinition.actions[0].attack)
			{
				string range = GetRangeString(item.itemDefinition);
				string AoE = GetAoEString(item.itemDefinition);
				string damage = GetDamageString(item.itemDefinition);
				if (AoE != "")
				{
					normal.stats.text = $"{range}, {AoE}, {damage}";
				}
				else
				{
					normal.stats.text = $"{range}, {damage}";
				}
			}
			normal.rules.text = item.itemDefinition.actions[0].actionDescription;
			Canvas.ForceUpdateCanvases();
			LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)normal.stats.transform.parent);
		}
	}

	public static string GetRangeString(ItemScriptableObject itemScriptable)
	{
		//range, line, cone, threat
		if (itemScriptable.actions[0].aoeType == ActionPattern.AoEType.Cone)
		{
			return $"cone {itemScriptable.actions[0].aoeRange}";
		}
		if (itemScriptable.actions[0].aoeType == ActionPattern.AoEType.Line)
		{
			return $"line {itemScriptable.actions[0].aoeRange}";
		}
		if (itemScriptable.actions[0].threatRange > 1)
		{
			return $"threat {itemScriptable.actions[0].threatRange}";
		}
		return $"range {itemScriptable.actions[0].range}";
	}

	public static string GetAoEString(ItemScriptableObject itemScriptable)
	{
		if (itemScriptable.actions[0].aoeType == ActionPattern.AoEType.Burst)
		{
			return $"blast {itemScriptable.actions[0].aoeRange}";
		}
		return "";
	}

	public static string GetDamageString(ItemScriptableObject itemScriptable)
	{
		List<(int face, int dice, int flat, ActionPattern.DamageType type)> damages = Util.ParseDiceString(itemScriptable.actions[0].damageString);
		if (damages[0].Item3 > 0)
		{
			if (damages[0].Item1 > 0)
			{
				return $"{damages[0].Item1}D{damages[0].Item2}+{damages[0].Item3}{Util.GetDamageTypeNameForDisplay(damages[0].type)}";
			}
			else
			{
				return $"{damages[0].Item3}{Util.GetDamageTypeNameForDisplay(damages[0].type)}";
			}
		}
		else if (damages[0].Item3 < 0)
		{
			return $"{damages[0].Item1}D{damages[0].Item2}-{-damages[0].Item3}{Util.GetDamageTypeNameForDisplay(damages[0].type)}";
		}
		else
		{
			return $"{damages[0].Item1}D{damages[0].Item2}{Util.GetDamageTypeNameForDisplay(damages[0].type)}";
		}
	}

	public static string GetActionKeywords(ItemScriptableObject itemScriptable, ActionPattern pattern)
	{
		if (pattern == null || pattern.keywords.Count == 0)
		{
			return itemScriptable.keywords;
		}
		string patternKeywords = string.Join(", ", pattern.keywords);
		return $"{itemScriptable.keywords}, {patternKeywords}";
	}

	public void SlotClicked(int index)
	{
		if (PersistenceManager.Instance.GetFlag("WeaponSlotsLocked"))
		{
			return;
		}
		AdvantageSelectionPane.Instance.Set(slotType, slot, item);
	}
}
