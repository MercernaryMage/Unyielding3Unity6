using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class KickN : ReactionBase
{

	public override void Execute()
	{
		if (!attackingCharacter.alive)
		{
			BattleController.ReturnControlToPlayer();
			return;
		}

		owningCharacter.SetFacing(TileGrid.Instance.GetFacingDirection(owningCharacter, attackingCharacter));
		ActionController.Instance.PlayAttackAnimation(owningCharacter, null, ReturnFromAnimation);
	}

	void ReturnFromAnimation()
	{
		Tile startTile = TileGrid.Instance.FindCharacter(attackingCharacter)[0];
		List<Tile> kickerTiles = TileGrid.Instance.FindCharacter(owningCharacter);

		int kickDistance = GetIntValue("Value");

		ActionController.Instance.KnockBack(attackingCharacter, kickerTiles, kickDistance);

		Tile endTile = TileGrid.Instance.FindCharacter(attackingCharacter)[0];
		int tilesMoved = Mathf.Max(Mathf.Abs(startTile.x - endTile.x), Mathf.Abs(startTile.y - endTile.y));
		int damage = kickDistance - tilesMoved;

		if (damage > 0 && attackingCharacter.alive)
		{
			ActionController.AttackResults results = new ActionController.AttackResults();
			results.fakeHit = true;
			ActionController.Instance.DamageCharacter(attackingCharacter, owningCharacter, new ActionController.AttackProfile(0, 0, damage), results);
		}

		BattleController.ReturnControlToPlayer();
	}

	public static List<CardInstruction> GetCardInstructions(CardScriptableObject scriptableObject)
	{
		DisplayGrid.Instance.Clear(12, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction($"Knock the attacker back {scriptableObject.GetTagIntValue("Value")} tiles"));
		instructions.Add(new CardInstruction("They take 1 damage for each tile they couldn't move"));
		return instructions;
	}
}
