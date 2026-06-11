using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TriggerDisplayElement : MonoBehaviour
{
    public TextMeshProUGUI triggerNameText;

	Trigger trigger;

	public void Set(Trigger t)
	{
		trigger = t;
		triggerNameText.text = t.GetTitle();
	}

	public void Click()
	{
		trigger.Click();
	}
}
