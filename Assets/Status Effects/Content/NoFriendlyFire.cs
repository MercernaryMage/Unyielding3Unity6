using System.Collections.Generic;
using UnityEngine;

public class NoFriendlyFire : StatusEffect
{
	//Pull this character's allies out of a hit list. Spends itself the first time it actually
	//saves someone, so it waits around until an attack would have caught a friend.
	public bool SpareAllies(List<Character> targets)
	{
		List<Character> spared = new List<Character>();
		foreach (Character target in targets)
		{
			if (target.hero == character.hero)
			{
				spared.Add(target);
			}
		}

		if (spared.Count == 0)
		{
			return false;
		}

		foreach (Character target in spared)
		{
			targets.Remove(target);
			FloatingCombatNumberController.Instance.QueueFloatingCombatNumber(target, "spared");
		}
		Destroy(this);
		return true;
	}

	public override string GetExplanationName()
	{
		return "No Friendly Fire";
	}

	public static List<CardInstruction> GetCardInstructions()
	{
		DisplayGrid.Instance.Clear(11, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction("The next time this character's <u>Blast</u> would hit an ally, it spares them instead."));

		return instructions;
	}
}
