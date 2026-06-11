using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactionBase : Card
{
    public Character attackingCharacter;

	public void Prep(Character attacker)
	{
		attackingCharacter = attacker;
	}
}
