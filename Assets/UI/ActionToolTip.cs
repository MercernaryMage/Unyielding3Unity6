using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class ActionToolTip : MonoBehaviour
{
    public TextMeshProUGUI titleText;
	public TextMeshProUGUI bodyText;
	ItemScriptableObject owningItem;

	public void Set(Character c, ActionPattern pattern, string reason, ItemScriptableObject i)
	{
		owningItem = i;
		
		if (!string.IsNullOrEmpty(reason))
		{
			titleText.text = pattern.displayName;
			bodyText.text = $"<b>{reason}</b>\n\n {pattern.actionDescription}";
			return;
		}
		if (pattern.attack)
		{
			ParseAttack(pattern);
			return;
		}

		titleText.text = pattern.displayName;
		if (!string.IsNullOrEmpty(pattern.actionDescriptionFunction))
		{
			bodyText.text = ActionTypes.GetDescription(c, pattern);
		}
		else
		{
			bodyText.text = pattern.actionDescription;
		}
	}

	public void ParseAttack(ActionPattern pattern)
	{
		titleText.text = owningItem.displayName;
		string outString = $"<size=20>{SlotDisplay.GetActionKeywords(owningItem, pattern)}</size>";
		
		string range = SlotDisplay.GetRangeString(owningItem);
		string AoE = SlotDisplay.GetAoEString(owningItem);
		string damage = SlotDisplay.GetDamageString(owningItem);
		if (AoE != "")
		{
			outString += $"\n{range}, {AoE}, {damage}";
		}
		else
		{
			outString += $"\n{range}, {damage}";
		}
		
		outString += $"\n{owningItem.actions[0].actionDescription}";

		bodyText.text = outString;
	}
}
