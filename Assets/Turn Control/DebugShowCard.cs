using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DebugShowCard : MonoBehaviour
{
	public string cardName;

	public void ShowCard()
	{
		if (cardName == "")
		{
			Debug.LogError("No card name to show");
			return;
		}

		CardScriptableObject cardScriptableObject = FindCardInPlay();
		if (cardScriptableObject == null)
		{
			cardScriptableObject = FindCardAsset();
		}

		if (cardScriptableObject == null)
		{
			Debug.LogError($"Could not find a card called {cardName}");
			return;
		}

		AICardDisplay.Instance.Dismiss(true);
		AICardDisplay.Instance.ShowCard(cardScriptableObject, CardDisplay.CardType.Card);
	}

	CardScriptableObject FindCardInPlay()
	{
		if (BattleController.Instance == null)
		{
			return null;
		}

		List<Character> characters = new List<Character>(BattleController.Instance.heroes);
		characters.AddRange(BattleController.Instance.enemies);

		foreach (Character character in characters)
		{
			List<Card> cards = new List<Card>(character.cards);
			cards.AddRange(character.cardDiscard);

			foreach (Card card in cards)
			{
				if (Matches(card.cardScriptableObject))
				{
					return card.cardScriptableObject;
				}
			}
		}

		return null;
	}

	CardScriptableObject FindCardAsset()
	{
#if UNITY_EDITOR
		foreach (string guid in AssetDatabase.FindAssets("t:CardScriptableObject"))
		{
			string path = AssetDatabase.GUIDToAssetPath(guid);
			CardScriptableObject cardScriptableObject = AssetDatabase.LoadAssetAtPath<CardScriptableObject>(path);
			if (Matches(cardScriptableObject))
			{
				return cardScriptableObject;
			}
		}
#endif
		return null;
	}

	bool Matches(CardScriptableObject cardScriptableObject)
	{
		if (cardScriptableObject == null)
		{
			return false;
		}
		if (string.Equals(cardScriptableObject.name, cardName, StringComparison.OrdinalIgnoreCase))
		{
			return true;
		}
		if (string.Equals(cardScriptableObject.className, cardName, StringComparison.OrdinalIgnoreCase))
		{
			return true;
		}
		return string.Equals(cardScriptableObject.cardDisplayName, cardName, StringComparison.OrdinalIgnoreCase);
	}
}
