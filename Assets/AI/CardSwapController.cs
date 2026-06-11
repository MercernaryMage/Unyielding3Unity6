using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSwapController : SceneSingleton<CardSwapController>
{
	public GameObject content;

    public CardDisplay cardDisplay1;
    public CardDisplay cardDisplay2;

	Reaction reaction1;
	Reaction reaction2;
	Character character;

	public void Set(Reaction r1, Reaction r2, Character c)
	{
		content.SetActive(true);
		reaction1 = r1;
		reaction2 = r2;
		character = c;
		Display();
	}

	void Display()
	{
		cardDisplay1.ShowCard(reaction1.GetCurrentCard().cardScriptableObject, false);
		cardDisplay2.ShowCard(reaction2.GetCurrentCard().cardScriptableObject, false);
		FadeLerp fadeLerp = content.AddComponent<FadeLerp>();
		fadeLerp.BasicFadeIn();
	}

    public void Swap()
	{
		Reaction temp = reaction2;
		reaction2 = reaction1;
		reaction1 = temp;
		Display();
	}

    public void Confirm()
	{
		character.reactions.RemoveAt(0);
		character.reactions.RemoveAt(0);
		character.reactions.Insert(0, reaction2);
		character.reactions.Insert(0, reaction1);
		ActionController.Instance.EndAction();
		FadeLerp fadeLerp = content.AddComponent<FadeLerp>();
		fadeLerp.BasicFadeOut();
		fadeLerp.callbackFunction = () => { content.SetActive(false); };
	}
}
