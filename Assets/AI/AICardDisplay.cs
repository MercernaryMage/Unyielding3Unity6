using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AICardDisplay : SceneSingleton<AICardDisplay>, IPointerEnterHandler
{
    public CardDisplay cardDisplay;
	float time = -1;
	public bool isShowing;

	public CardScriptableObject cardForDebug;

	public void ShowFakeCard(string title, string body)
	{
		isShowing = true;
		cardDisplay.ShowFakeCard(title, body);
	}

	public void ShowCard(CardScriptableObject cardScriptableObject)
	{
		isShowing = true;
		cardDisplay.ShowCard(cardScriptableObject, true);
	}

	public void SoftDismiss(float t)
	{
		time = t;
	}

	public void Dismiss()
	{
		if (isShowing)
		{
			isShowing = false;
			cardDisplay.Dismiss();
		}
	}

	private void Update()
	{
		if (time > 0)
		{
			time -= Time.deltaTime;
			if (time < 0)
			{
				Dismiss();
			}
		}	
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		time = -1;
	}

	public void ShowCardDebug()
	{

		Type t = Type.GetType(cardForDebug.className);
		Card c = (Card)Activator.CreateInstance(t);
		c.Set(cardForDebug);
		ShowCard(c.cardScriptableObject);
	}
}
