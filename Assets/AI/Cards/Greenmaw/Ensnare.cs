using System;
using System.Collections.Generic;
using UnityEngine;

public class Ensnare : Card
{
	Tuple<List<Tile>, Tile> route;
	List<Tile> litRouteTiles;
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
			AnimationController.Instance.ScrollToCharacter(targetHero, ApplyParalyzed, .5f);
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

	void ApplyParalyzed()
	{
		targetHero.AddStatusEffect(typeof(Paralyzed), null);
		AnimationController.Instance.DelayedCallback(1.0f, () => Finish());
	}

	void DoMove()
	{
		//No target in range: close on the nearest enemy that is not already paralyzed.
		Dictionary<Character, Tuple<List<Tile>, Tile>> routes = RouteToAllClosestCharacters(true);
		route = Util.FindSmallestRoute(routes, IsNotParalyzed);

		if (route == null)
		{
			PlayTrap();
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
			PlayTrap();
			return;
		}
		AnimationController.Instance.ScrollToCharacter(targetHero, ApplyParalyzed, .5f);
	}

	void PlayTrap()
	{
		Util.BringCardToTopOfDeck(owningCharacter, "Trap");
		AIController.Instance.TakeTurn(owningCharacter);
	}

	public static List<CardInstruction> GetCardInstructions(CardScriptableObject scriptableObject)
	{
		DisplayGrid.Instance.Clear(11, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction("Apply <u>Paralyzed</u> to an enemy within range 3 that is not <u>Paralyzed</u>"));
		instructions.Add(new CardInstruction("If there is no target, move toward the closest enemy that is not <u>Paralyzed</u> and try again"));
		instructions.Add(new CardInstruction("If there is still no target, play Trap"));
		DisplayGrid.Instance.Show();

		return instructions;
	}
}
