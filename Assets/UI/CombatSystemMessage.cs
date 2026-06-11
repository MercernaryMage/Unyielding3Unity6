using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CombatSystemMessage : SceneSingleton<CombatSystemMessage>
{
    public TextMeshProUGUI titleText;
	public TextMeshProUGUI bodyText;
	public GameObject content;
	public bool isShowing = false;

	public void Show(string title, string body)
	{
		titleText.gameObject.SetActive(title != "");
		titleText.text = title;
		bodyText.gameObject.SetActive(body != "");
		bodyText.text = body;
		content.SetActive(true);
		isShowing = true;
		FadeLerp lerp =  content.AddComponent<FadeLerp>();
		lerp.BasicFadeIn();
	}

	public void Click()
	{
		FadeLerp lerp = content.AddComponent<FadeLerp>();
		lerp.BasicFadeOut();
		lerp.callbackFunction = ClickActual;
		isShowing = false;
	}

	public void ClickActual()
	{
		content.SetActive(false);
	}
}
