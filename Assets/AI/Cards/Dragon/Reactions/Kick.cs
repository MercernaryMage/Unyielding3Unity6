using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Kick : ReactionBase
{
	Tuple<List<Tile>, Tile> route;

	public override void Execute()
	{
		if (!attackingCharacter.alive)
		{
			BattleController.ReturnControlToPlayer();
			return;
		}

		Dictionary<Character, Tuple<List<Tile>, Tile>> routes = RouteToAllClosestCharacters(true);

		if (!routes.ContainsKey(attackingCharacter) ||
			routes[attackingCharacter] == null ||
			routes[attackingCharacter].Item1.Count > owningCharacter.characterDefinition.movement + 1)
		{
			FloatingCombatNumberController.Instance.QueueFloatingCombatNumber(owningCharacter, "out of range");
			BattleController.ReturnControlToPlayer();
			return;
		}

		route = routes[attackingCharacter];
		AnimationController.Instance.ShowTiles(route.Item1, Tile.OverlayType.PossibleMovement, ReturnFromShowingTiles, AttackTarget);
	}

	void ReturnFromShowingTiles()
	{
		// Move to the attacker without provoking attacks of opportunity.
		TileGrid.Instance.RouteAICharacterToTile(owningCharacter, new List<Tile>(route.Item1), AttackTarget, true, false, true);
	}

	void AttackTarget()
	{
		foreach (Tile t in route.Item1)
		{
			t.HideOverlay(Tile.OverlayType.PossibleMovement);
		}

		if (!owningCharacter.alive || !attackingCharacter.alive)
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
			CombatLogControl.Instance.AddEntry($"Kick deals {results.damageDealt} to {attackingCharacter.displayName}");
		}

		BattleController.ReturnControlToPlayer();
	}

	public static List<CardInstruction> GetCardInstructions(CardScriptableObject scriptableObject)
	{
		DisplayGrid.Instance.Clear(12, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction("Move to the attacker if in range"));
		instructions.Add(new CardInstruction($"Knock the attacker back {scriptableObject.GetTagIntValue("Value")} tiles"));
		instructions.Add(new CardInstruction("They take 1 damage for each tile they couldn't move"));
		return instructions;
	}
}
