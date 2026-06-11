using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TownSystemPopup : MonoBehaviour
{
	public GameObject content;
	public TextMeshProUGUI systemText;

	static string queuedMessage = "";

	void Start()
	{
		if (queuedMessage != "")
		{
			Set(queuedMessage);
			queuedMessage = "";
		}
	}

	public void Set(string text)
	{
		content.SetActive(true);
		systemText.text = text;
	}

	public void Dismiss()
	{
		content.SetActive(false);
	}

	static public void QueueMessage(string message)
	{
		queuedMessage = message;
	}
}
