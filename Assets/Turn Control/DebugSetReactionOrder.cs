using System.Collections.Generic;
using UnityEngine;

public class DebugSetReactionOrder : MonoBehaviour
{
	public string characterName;
	public List<string> reactionNames = new List<string>();

	void Start()
	{
		if (!Application.isEditor)
		{
			return;
		}
		Invoke("ApplyReactionOrder", 1);
	}

	void ApplyReactionOrder()
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

		List<Reaction> remaining = new List<Reaction>(found.reactions);
		List<Reaction> ordered = new List<Reaction>();

		//Pull out the reactions named in reactionNames, in that order. Names that match
		//nothing are skipped; reactions not named are left for the end.
		foreach (string reactionName in reactionNames)
		{
			for (int i = remaining.Count - 1; i >= 0; --i)
			{
				if (Matches(remaining[i], reactionName))
				{
					ordered.Add(remaining[i]);
					remaining.RemoveAt(i);
				}
			}
		}

		ordered.AddRange(remaining);
		found.reactions = ordered;
	}

	bool Matches(Reaction reaction, string reactionName)
	{
		Card card = reaction.GetCurrentCard();
		if (card == null || card.cardScriptableObject == null)
		{
			return false;
		}
		return card.cardScriptableObject.className == reactionName ||
			card.cardScriptableObject.cardDisplayName == reactionName;
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
