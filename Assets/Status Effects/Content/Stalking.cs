using System.Collections.Generic;
using UnityEngine;

public class Stalking : StatusEffect
{
	public Stalk stalkCard;
	public Character target;
	public Stalked stalked;

	public void Set(Stalk card, Character stalkTarget, Stalked newStalked)
	{
		stalkCard = card;
		target = stalkTarget;
		stalked = newStalked;
	}

	public override void CharacterStartTurn(CharacterStartTurnMessage message)
	{
		if (!character.alive)
		{
			Cleanup();
			return;
		}
		if (message.character != character)
		{
			return;
		}
		if (stalkCard == null || target == null || !target.alive || target.IsDowned())
		{
			Cleanup();
			return;
		}

		message.turnStartLocks.Add(this);
		BattleController.playerHasControl = false;

		stalkCard.ChaseTarget(target, () =>
		{
			BattleController.playerHasControl = true;
			Cleanup();
			TurnControl.Instance.RemoveLock(this);
		});
	}

	void Cleanup()
	{
		if (stalked != null)
		{
			stalked.Remove();
			stalked = null;
		}
		Destroy(this);
	}

	public override void EffectBeingRemoved()
	{
		if (stalked != null)
		{
			stalked.Remove();
			stalked = null;
		}
	}

	public override string GetExplanationName()
	{
		return "Stalking";
	}

	public static List<CardInstruction> GetCardInstructions()
	{
		DisplayGrid.Instance.Clear(11, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction("At the start of this character's turn, it moves to the marked enemy and attacks it."));

		return instructions;
	}
}
