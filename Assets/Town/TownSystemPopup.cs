using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TownSystemPopup : MonoBehaviour
{
	public GameObject content;
	public TextMeshProUGUI systemText;
	public GameObject questButton;
	public GameObject weaponSlot;
	public GameObject addButton;

	static string queuedMessage = "";
	public static bool pulseQuestButton = false;
	public static bool pulseWeaponSlot = false;
	public static bool pulseAddButton = false;

	void Start()
	{
		if (queuedMessage != "")
		{
			Set(queuedMessage);
			queuedMessage = "";
		}
		if (pulseQuestButton)
		{
			Pulse pulse = questButton.AddComponent<Pulse>();
			pulse.scale = .1f;
			pulseQuestButton = false;
			TownManager.Instance.hasWarnedPlayer = true;
		}
		if (pulseAddButton)
		{
			Pulse pulse = addButton.AddComponent<Pulse>();
			pulse.scale = .3f;
			pulseQuestButton = false;
		}
		if (pulseWeaponSlot)
		{
			pulseWeaponSlot = false;
			Pulse pulse = weaponSlot.AddComponent<Pulse>();
			pulse.scale = .1f;

			RemovePulse removePulse = weaponSlot.AddComponent<RemovePulse>();
			removePulse.pulse = pulse;
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
