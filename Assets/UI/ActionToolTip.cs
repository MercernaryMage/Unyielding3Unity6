using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ActionToolTip : MonoBehaviour
{
    public TextMeshProUGUI titleText;
	public TextMeshProUGUI bodyText;
	public ActionTooltipBubbleGenerator bubbleGenerator;
	ItemScriptableObject owningItem;

	public void Set(Character c, ActionPattern pattern, string reason, bool usable, ItemScriptableObject i)
	{
		owningItem = i;

		if (pattern.attack)
		{
			ParseAttack(pattern);
		}
		else
		{
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

		if (!string.IsNullOrEmpty(reason))
		{
			bodyText.text = $"<b>{reason}</b>\n\n{bodyText.text}";
		}
		Canvas.ForceUpdateCanvases();
		LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)bodyText.transform.parent);
	}

	public void ParseAttack(ActionPattern pattern)
	{
		titleText.text = owningItem.displayName;
		bubbleGenerator.Create(SlotDisplay.GetActionKeywordsAsList(owningItem, pattern));
		string outString = "";
		string range = SlotDisplay.GetRangeString(owningItem);
		string AoE = SlotDisplay.GetAoEString(owningItem);
		string damage = SlotDisplay.GetDamageString(owningItem);
		if (AoE != "")
		{
			outString += $"{range}, {AoE},\n{damage}";
		}
		else
		{
			outString += $"{range},\n{damage}";
		}
		
		outString += $"\n{owningItem.actions[0].actionDescription}";

		bodyText.text = outString;
	}
}
