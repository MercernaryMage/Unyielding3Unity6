using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.UI;
using System;

public class CombatLogControl : SceneSingleton<CombatLogControl>
{
	public GameObject entryPrefab;
	public Transform targetTransform;
	public ScrollRect scrollRect;
	public TextMeshProUGUI popoutText;
	public GameObject popout;

	private void Start()
	{
	}

	public void AddEntry(string s)
	{
		AddEntry(s, new List<string>());
	}

	public void AddEntry(string s, List<string> lines)
	{
		lines.RemoveAll(line => line == "");
		GameObject obj = Instantiate(entryPrefab);

		CombatLogEntry log = obj.GetComponent<CombatLogEntry>();
		log.Set(s, lines, false, null);

		obj.transform.SetParent(targetTransform);
		Canvas.ForceUpdateCanvases();
		LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)targetTransform.parent);
		scrollRect.verticalNormalizedPosition = 0;
	}

	public void AddCard(FormattableString s, Card c)
	{
		AddCardToFormat(s, c);
	}

	public void AddFormattedCard(string s, Card c)
	{
		GameObject obj = Instantiate(entryPrefab);

		CombatLogEntry log = obj.GetComponent<CombatLogEntry>();
		log.Set(s, null, true, c.cardScriptableObject);

		obj.transform.SetParent(targetTransform);
		Canvas.ForceUpdateCanvases();
		LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)targetTransform.parent);
		scrollRect.verticalNormalizedPosition = 0;
	}

	public void ShowPopout(string text)
	{
		popout.SetActive(true);
		popoutText.text = text;
		Canvas.ForceUpdateCanvases();
		LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)targetTransform.parent);
	}

	public void HidePopout()
	{
		popout.SetActive(false);
	}

	 
	public void AddCardToFormat(FormattableString fmtStr, Card c)
    {
        AddFormattedCard(Format(fmtStr), c);
    }

    static string Format(FormattableString fmtStr)
    {
        object[] args = new object[fmtStr.ArgumentCount];
        for (int i = 0; i < fmtStr.ArgumentCount; i++)
        {
            args[i] = FormatEntry(fmtStr.GetArgument(i));
        }

        return string.Format(fmtStr.Format, args);
    }

    static string FormatEntry(object entry)
    {
        if (entry is Card card)
        {
            string calloutString = card.cardScriptableObject.cardDisplayName;
            return $"<u>{calloutString}</u>";
        }

        if (entry is Character character)
        {
            string characterString = character.displayName;
            return $"<u>{characterString}</u>";
        }

        if (entry is string str)
        {
            return str;
        }

        if (entry is int i)
        {
            return i.ToString();
        }

        if (entry is null)
        {
            Debug.LogWarning("Null! FormatEntry argument");
            return "???";
        }

        Debug.LogWarning($"Unhandled FormatEntry type: {entry.GetType().FullName}");
        return entry.ToString();
    }
	 
}
