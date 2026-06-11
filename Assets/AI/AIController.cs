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
			AICardDisplay.Instance.Dismiss();
			AICardDisplay.Instance.ShowCard(character.cards[0].cardScriptableObject);
			CardStartMessage cardStartMessage = new CardStartMessage();
			cardStartMessage.character = character;
			cardStartMessage.card = character.cards[0];
			MessagePump.Instance.SendMessage(cardStartMessage);
			CombatLogControl.Instance.AddCard($"{character.displayName} uses {character.cards[0].GetCardName()}", character.cards[0]);
			character.cards[0].Execute();
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

	void ReshuffleReactions(Character c)
	{
		c.reactions.AddRange(c.reactionDiscard);
		c.reactionDiscard.Clear();
		Util.Shuffle(c.reactions);
	}

	public void DoReaction(Character reactingCharacter, Character attacker)
	{
		BattleController.RemoveControlFromPlayer(HeroDisplayRouter.Instance.mainDisplay.lastCharacter);
		reactingCharacter.reactions[0].Prep(attacker);
		reactingCharacter.reactions[0].Execute();
		Reaction reaction = reactingCharacter.reactions[0];
		reactingCharacter.reactions.RemoveAt(0);
		reactingCharacter.reactionDiscard.Add(reaction);
		if (reactingCharacter.reactions.Count == 0)
		{
			ReshuffleReactions(reactingCharacter);
		}
		CombatLogControl.Instance.AddCard($"{reactingCharacter} reacts with {reaction.GetCurrentCard()}", reaction.GetCurrentCard());
		AICardDisplay.Instance.ShowCard(reaction.GetCurrentCard().cardScriptableObject);
	}
}
