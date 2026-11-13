using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TriggerDisplayElement : MonoBehaviour
{
    public TextMeshProUGUI triggerNameText;
	public Image characterHead;

	Trigger trigger;

	public void Set(Trigger t)
	{
		trigger = t;
		triggerNameText.text = t.GetTitle();
		characterHead.sprite = t.owningCharacter.characterDefinition.characterSmallHead;
	}

	public void Click()
	{
		trigger.Click();
	}
}
