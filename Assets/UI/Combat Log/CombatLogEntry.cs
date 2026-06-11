using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CombatLogEntry : MonoBehaviour, IPointerEnterHandler , IPointerExitHandler
{
    public TextMeshProUGUI text;
	List<string> entries;
	CardScriptableObject cardScriptableObject;
	bool card;
	string totalText = "";

	public void Set(string t, List<string> e, bool c, CardScriptableObject s)
	{
		text.text = t;
		entries = e;
		card = c;
		cardScriptableObject = s;
		if (e != null && e.Count > 0)
		{
			totalText = string.Join("\n", e);
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (card)
		{
			AICardDisplay.Instance.ShowCard(cardScriptableObject);
		}
		else
		{
			if (totalText != "")
			{
				CombatLogControl.Instance.ShowPopout(totalText);
			}
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (card)
		{
			AICardDisplay.Instance.SoftDismiss(.5f);
		}
		else
		{
			CombatLogControl.Instance.HidePopout();
		}
	}
}
