using System.Collections.Generic;
using UnityEngine;

public class DebugSetCardOrder : MonoBehaviour
{
	public string characterName;
	public List<string> cardNames = new List<string>();

	void Start()
	{
		if (!Application.isEditor)
		{
			return;
		}
		Invoke("ApplyCardOrder", 1);
	}

	void ApplyCardOrder()
	{
		if (string.IsNullOrEmpty(characterName))
		{
			return;
		}
		Character found = FindCharacter(characterName);
		if (found == null)
		{
			return;
		}

		List<Card> remaining = new List<Card>(found.cards);
		List<Card> ordered = new List<Card>();

		//Pull out the cards named in cardNames, in that order. Names that match nothing are
		//skipped; cards not named are left for the end.
		foreach (string cardName in cardNames)
		{
			for (int i = remaining.Count - 1; i >= 0; --i)
			{
				if (Matches(remaining[i], cardName))
				{
					ordered.Add(remaining[i]);
					remaining.RemoveAt(i);
				}
			}
		}

		ordered.AddRange(remaining);
		found.cards = ordered;
	}

	bool Matches(Card card, string cardName)
	{
		if (card == null || card.cardScriptableObject == null)
		{
			return false;
		}
		return card.cardScriptableObject.className == cardName ||
			card.cardScriptableObject.cardDisplayName == cardName;
	}

	Character FindCharacter(string name)
	{
		foreach (Character c in BattleController.Instance.heroes)
		{
			if (c.displayName == name)
			{
				return c;
			}
		}
		foreach (Character c in BattleController.Instance.enemies)
		{
			if (c.displayName == name)
			{
				return c;
			}
		}
		return null;
	}
}
