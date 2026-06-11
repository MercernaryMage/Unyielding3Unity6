using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Reaction
{
    public ReactionBase normalReaction;
    public ReactionBase aggravatedReaction;

    public bool isAggravatged = false;

	public void Prep(Character attacker)
	{
		if (isAggravatged && aggravatedReaction != null)
		{
			aggravatedReaction.Prep(attacker);
		}
		else
		{
			normalReaction.Prep(attacker);
		}

	}

	public void Execute()
	{
		if (isAggravatged && aggravatedReaction != null)
		{
			aggravatedReaction.Execute();
		}
		else
		{
			normalReaction.Execute();
		}
	}

	public Card GetCurrentCard()
	{
		if (isAggravatged && aggravatedReaction != null)
		{
			return aggravatedReaction;
		}
		else
		{
			return normalReaction;
		}

	}
}
