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

	CardScriptableObject lastShownCard;
	Action pendingResolve;

	public CardScriptableObject cardForDebug;

	public void ShowFakeCard(string title, string body)
	{
		isShowing = true;
		cardDisplay.ShowFakeCard(title, body);
	}

	public void ShowCardAndWaitForDismiss(CardScriptableObject cardScriptableObject, CardDisplay.CardType cardType, Character character, Action onDismissed)
	{
		pendingResolve = null;
		ShowCardActual(cardScriptableObject, cardType, character, true);
		pendingResolve = onDismissed;
	}

	public void CardDismissed()
	{
		if (pendingResolve == null)
		{
			return;
		}
		Action resolve = pendingResolve;
		pendingResolve = null;
		resolve();
	}

	public void ShowCard(CardScriptableObject cardScriptableObject, CardDisplay.CardType cardType, Character character = null)
	{
		if (pendingResolve != null)
		{
			return;
		}
		ShowCardActual(cardScriptableObject, cardType, character, false);
	}

	void ShowCardActual(CardScriptableObject cardScriptableObject, CardDisplay.CardType cardType, Character character, bool waitingForDismiss)
	{
		if (time > 0 && cardScriptableObject == lastShownCard && isShowing)
		{
			time = -1;
			return;
		}
		time = -1;
		lastShownCard = cardScriptableObject;
		isShowing = true;
		cardDisplay.ShowCard(cardScriptableObject, true, cardType, character, waitingForDismiss);
	}

	public void SoftDismiss(float t)
	{
		if (pendingResolve != null)
		{
			return;
		}
		time = t;
	}

	public void Dismiss(bool fly)
	{
		if (isShowing)
		{
			isShowing = false;
			cardDisplay.Dismiss(fly);
		}
	}

	private void Update()
	{
		if (time > 0)
		{
			time -= Time.deltaTime;
			if (time < 0)
			{
				Dismiss(false);
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
		ShowCard(c.cardScriptableObject, CardDisplay.CardType.Card);
	}
}
