using System;
using System.Collections.Generic;
using UnityEngine;

public class Poke : Card
{
	Tuple<List<Tile>, Tile> route;
	List<Tile> litRouteTiles;
	List<Tile> attackTiles;
	Character targetHero;

	int pokeRange = 2;

	//Move towards the closest enemy but stop one space short of contact, then attack from there

	public override void Execute()
	{
		Dictionary<Character, Tuple<List<Tile>, Tile>> routes = RouteToAllClosestCharacters(true);

		KeyValuePair<Character, Tuple<List<Tile>, Tile>> closest = Util.FindSmallestRoutePair(routes, null);
		targetHero = closest.Key;
		route = closest.Value;

		if (route == null)
		{
			Debug.Log("No possible route");
			Finish();
			return;
		}

		//The route heads for contact, so cut it as soon as it reaches poking distance.
		Util.ShortenPathToDesiredRange(route, owningCharacter, targetHero, pokeRange);
		Util.ShortenPathToMaxRange(route, owningCharacter.characterDefinition.movement + 1);

		litRouteTiles = Util.ExpandPathTiles(route.Item1, owningCharacter);
		AnimationController.Instance.ShowTiles(litRouteTiles, Tile.OverlayType.PossibleMovement, Route, ReturnFromRoute);
	}

	public void Route()
	{
		TileGrid.Instance.RouteAICharacterToTile(owningCharacter, new List<Tile>(route.Item1), ReturnFromRoute);
	}

	public void ReturnFromRoute()
	{
		foreach (Tile t in litRouteTiles)
		{
			t.HideOverlay(Tile.OverlayType.PossibleMovement);
		}

		//Movement can run out before the target is in reach, so only poke if the range worked out.
		if (!targetHero.alive || TileGrid.Instance.GetDistanceBetweenCharacters(owningCharacter, targetHero) > pokeRange)
		{
			Debug.Log("No targets in range");
			Finish();
			return;
		}

		attackTiles = TileGrid.Instance.FindCharacter(targetHero);
		AnimationController.Instance.ShowTiles(attackTiles, Tile.OverlayType.PossibleAttck, ReturnFromShowingAttackTiles);
	}

	public void ReturnFromShowingAttackTiles()
	{
		owningCharacter.SetFacing(TileGrid.Instance.GetFacingDirection(owningCharacter, targetHero));
		ActionController.Instance.PlayAttackAnimation(owningCharacter, null, () =>
		{
			ActionController.Instance.AttackCharacter(targetHero, owningCharacter, new ActionController.AttackProfile(1, 6, 0));

			foreach (Tile t in attackTiles)
			{
				t.HideOverlay(Tile.OverlayType.PossibleAttck);
			}

			Finish();
		});
	}

	public static List<CardInstruction> GetCardInstructions(CardScriptableObject scriptableObject)
	{
		DisplayGrid.Instance.Clear(11, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction("Move towards the closest enemy, stopping 2 away"));
		instructions.Add(new CardInstruction());
		instructions.Add(new CardInstruction("Attack that enemy for 1d6 damage"));

		DisplayGrid.Instance.Add(DisplayGrid.DisplayGridObject.Size1Enemy, DisplayGrid.DisplayGridDirection.East, 4, 4);
		DisplayGrid.Instance.Add(DisplayGrid.DisplayGridObject.EffectedTile, new List<Tuple<int, int>>()
		{
			new Tuple<int, int>(6, 4),
		});
		DisplayGrid.Instance.Show();

		return instructions;
	}
}
