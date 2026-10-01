using System;
using System.Collections.Generic;
using UnityEngine;

public class Ensnare : Card
{
	Tuple<List<Tile>, Tile> route;
	List<Tile> litRouteTiles;
	List<Tile> attackTiles;
	Character targetHero;

	const int range = 3;

	bool IsNotParalyzed(Character target)
	{
		if (target.GetComponent<Downed>())
		{
			return false;
		}
		return target.GetComponent<Paralyzed>() == null;
	}

	public override void Execute()
	{
		targetHero = GetTarget();
		if (targetHero != null)
		{
			AnimationController.Instance.ScrollToCharacter(targetHero, ShowAttackTiles, .5f);
			return;
		}

		DoMove();
	}

	//A valid target is an enemy in range with line of sight that is not already paralyzed.
	Character GetTarget()
	{
		List<Character> heroesInRange = Util.GetHeroesInRange(owningCharacter, range);
		heroesInRange.RemoveAll(o => !TileGrid.Instance.DoesCharacterHaveLOSToCharacter(o, owningCharacter));
		heroesInRange.RemoveAll(o => o.GetComponent<Paralyzed>() != null);
		if (heroesInRange.Count == 0)
		{
			return null;
		}

		return heroesInRange[UnityEngine.Random.Range(0, heroesInRange.Count)];
	}

	void ShowAttackTiles()
	{
		attackTiles = TileGrid.Instance.FindCharacter(targetHero);
		AnimationController.Instance.ShowTiles(attackTiles, Tile.OverlayType.PossibleAttck, Attack);
	}

	void Attack()
	{
		owningCharacter.SetFacing(TileGrid.Instance.GetFacingDirection(owningCharacter, targetHero));
		ActionController.Instance.PlayAttackAnimation(owningCharacter, null, () =>
		{
			ActionController.AttackResults results = ActionController.Instance.AttackCharacter(
				targetHero, owningCharacter, new ActionController.AttackProfile(1, 6, 0, true));
			if (results.hit)
			{
				targetHero.AddStatusEffect(typeof(Paralyzed), null);
			}
			foreach (Tile t in attackTiles)
			{
				t.HideOverlay(Tile.OverlayType.PossibleAttck);
			}

			AnimationController.Instance.DelayedCallback(1.0f, () => Finish());
		});
	}

	void DoMove()
	{
		//No target in range: close on the nearest enemy that is not already paralyzed.
		Dictionary<Character, Tuple<List<Tile>, Tile>> routes = RouteToAllClosestCharacters(true);
		route = Util.FindSmallestRoute(routes, IsNotParalyzed);

		if (route == null)
		{
			Finish();
			return;
		}

		Util.ShortenPathToMaxRange(route, owningCharacter.characterDefinition.movement + 1);

		litRouteTiles = Util.ExpandPathTiles(route.Item1, owningCharacter);
		AnimationController.Instance.ShowTiles(litRouteTiles, Tile.OverlayType.PossibleMovement, ReturnFromShowingTiles, ReturnFromMove);
	}

	void ReturnFromShowingTiles()
	{
		TileGrid.Instance.RouteAICharacterToTile(owningCharacter, new List<Tile>(route.Item1), ReturnFromMove);
	}

	void ReturnFromMove()
	{
		foreach (Tile t in litRouteTiles)
		{
			t.HideOverlay(Tile.OverlayType.PossibleMovement);
		}

		//Try the targeting logic once more from our new position.
		targetHero = GetTarget();
		if (targetHero == null)
		{
			Finish();
			return;
		}
		AnimationController.Instance.ScrollToCharacter(targetHero, ShowAttackTiles, .5f);
	}

	public static List<CardInstruction> GetCardInstructions(CardScriptableObject scriptableObject)
	{
		DisplayGrid.Instance.Clear(11, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction("Attack an enemy within range 3 that is not <u>Paralyzed</u> for 1d6 damage"));
		instructions.Add(new CardInstruction("On hit, apply <u>Paralyzed</u>"));
		instructions.Add(new CardInstruction("If there is no target, move toward the closest enemy that is not <u>Paralyzed</u> and attack"));
		DisplayGrid.Instance.Show();

		return instructions;
	}
}
