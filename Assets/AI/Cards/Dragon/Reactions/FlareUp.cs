using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FlareUp : ReactionBase
{
	public override void Execute()
	{
		ActionController.Instance.PlayAttackAnimation(owningCharacter, null, ReturnFromAnimation);
	}

	void ReturnFromAnimation()
	{
		int range = GetIntValue("Range");
		List<Tile> myTiles = TileGrid.Instance.FindCharacter(owningCharacter);

		List<Character> allCharacters = new List<Character>(BattleController.Instance.heroes);
		allCharacters.AddRange(BattleController.Instance.enemies);

		foreach (Character c in allCharacters)
		{
			if (c == owningCharacter || !c.alive)
			{
				continue;
			}

			if (TileGrid.Instance.GetDistanceBetweenCharacters(c, owningCharacter) > range)
			{
				continue;
			}

			ActionController.AttackResults results = new ActionController.AttackResults();
			results.fakeHit = true;
			ActionController.AttackProfile profile = new ActionController.AttackProfile(0, 0, 2);
			profile.damageType = ActionPattern.DamageType.Burning;
			ActionController.Instance.DamageCharacter(c, owningCharacter, profile, results);
		}

		BattleController.ReturnControlToPlayer();
	}

	public static List<CardInstruction> GetCardInstructions(CardScriptableObject scriptableObject)
	{
		DisplayGrid.Instance.Clear(12, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction($"Deal 2 burning damage to all characters within range {scriptableObject.GetTagIntValue("Range")}"));
		return instructions;
	}
}
