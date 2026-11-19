using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : SceneSingleton<AIController>
{
	int index;

	public void TakeTurn(Character character)
	{
		if (character.GetComponent<Stun>() != null)
		{
			BattleController.playerHasControl = true;
			TurnControl.Instance.Pump();
			return;
		}
		if (character.alive && (character.cards.Count > 0 || character.cardDiscard.Count > 0))
		{
			DiscardSilentFailures(character);
			AICardDisplay.Instance.Dismiss(true);
			CardScriptableObject cardScriptableObject = character.cards[0].cardScriptableObject;
			if (PersistenceManager.Instance.HasCardBeenPlayed(cardScriptableObject))
			{
				AICardDisplay.Instance.ShowCard(cardScriptableObject, CardDisplay.CardType.Card, character);
				ResolveCard(character);
			}
			else
			{
				AICardDisplay.Instance.ShowCardAndWaitForDismiss(cardScriptableObject, CardDisplay.CardType.Card, character, () => ResolveCard(character));
			}
		}
	}

	void ResolveCard(Character character)
	{
		PersistenceManager.Instance.MarkCardPlayed(character.cards[0].cardScriptableObject);
		CardStartMessage cardStartMessage = new CardStartMessage();
		cardStartMessage.character = character;
		cardStartMessage.card = character.cards[0];
		MessagePump.Instance.SendMessage(cardStartMessage);
		CombatLogControl.Instance.AddCard($"{character.displayName} uses {character.cards[0].GetCardName()}", character.cards[0]);
		character.cards[0].Execute();
	}

	void DiscardSilentFailures(Character character)
	{
		int remaining = character.cards.Count + character.cardDiscard.Count;
		while (remaining > 0 && character.cards.Count > 0 && character.cards[0].FailSilently())
		{
			Reshuffle(character);
			--remaining;
		}
	}

	public void Reshuffle(Character c)
	{
		Card card = c.cards[0];
		card.isRevealed = false;
		c.cards.RemoveAt(0);
		c.cardDiscard.Add(card);
		if (c.cards.Count == 0)
		{
			ReshuffleDeck(c);
		}
	}

	void ReshuffleDeck(Character c)
	{
		c.cards.AddRange(c.cardDiscard);
		c.cardDiscard.Clear();
		Util.Shuffle(c.cards);
	}

	public void DoReaction(Character reactingCharacter, Character attacker)
	{
		BattleController.RemoveControlFromPlayer(UIController.Instance.heroDisplay.lastCharacter);
		reactingCharacter.reactions[0].Prep(attacker);
		reactingCharacter.reactions[0].Execute();
		Reaction reaction = reactingCharacter.reactions[0];
		reactingCharacter.reactions.RemoveAt(0);
		reactingCharacter.reactions.Add(reaction);
		CombatLogControl.Instance.AddCard($"{reactingCharacter} reacts with {reaction.GetCurrentCard()}", reaction.GetCurrentCard());
		AICardDisplay.Instance.ShowCard(reaction.GetCurrentCard().cardScriptableObject, reaction.isAggravatged? CardDisplay.CardType.AggravatedReaction : CardDisplay.CardType.Reaction, reactingCharacter);
	}
}
