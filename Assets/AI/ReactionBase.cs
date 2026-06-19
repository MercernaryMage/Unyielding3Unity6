using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ReactionBase : Card
{
    public Character attackingCharacter;

	public void Prep(Character attacker)
	{
		attackingCharacter = attacker;
	}
}
