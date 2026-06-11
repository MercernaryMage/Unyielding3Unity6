using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static ActionController;

public class HookAndPull : Card
{
	TemplateLibrary.TilesAndDirection tilesAndDirection;
	Tuple<List<Tile>, Tile> route;
	Character targetHero;

	//Move to range 3, attack, then pull target close

	public override void Execute()
	{
		Dictionary<Character, Tuple<List<Tile>, Tile>> routes = RouteToAllClosestCharacters(true);

		targetHero = null;
		route = null;
		foreach (KeyValuePair<Character, Tuple<List<Tile>, Tile>> pair in routes)
		{
			if (!pair.Key.alive) continue;
			if (route == null || route.Item1.Count > pair.Value.Item1.Count)
			{
				route = pair.Value;
				targetHero = pair.Key;
			}
		}

		if (route == null)
		{
			Debug.Log("No possible route");
			Finish();
			return;
		}

		Util.ShortenPathToDesiredRange(route, owningCharacter, targetHero, 3);
		Util.ShortenPathToMaxRange(route, owningCharacter.characterDefinition.movement + 1);

		AnimationController.Instance.ShowTiles(route.Item1, Tile.OverlayType.PossibleMovement, Route, ReturnFromRoute);
	}

	public void Route()
	{
		TileGrid.Instance.RouteCharacterToTile(owningCharacter, new List<Tile>(route.Item1), ReturnFromRoute);
	}

	public void ReturnFromRoute()
	{
		foreach (Tile t in route.Item1)
		{
			t.HideOverlay(Tile.OverlayType.PossibleMovement);
		}

		targetHero = Util.FindFarthestHeroWithinRange(3, owningCharacter);
		if (targetHero == null)
		{
			Debug.Log("No targets in range");
			Finish();
			return;
		}

		List<Tile> tiles = TileGrid.Instance.FindCharacter(targetHero);
		tilesAndDirection = new TemplateLibrary.TilesAndDirection(tiles, Direction.East);
		AnimationController.Instance.ShowTiles(tiles, Tile.OverlayType.PossibleAttck, ReturnFromShowingAttackTiles);
	}

	public void ReturnFromShowingAttackTiles()
	{
		owningCharacter.SetFacing(TileGrid.Instance.GetFacingDirection(owningCharacter, targetHero));
		ActionController.Instance.PlayAttackAnimation(owningCharacter, null, () =>
		{
			AttackResults results = ActionController.Instance.AttackCharacter(targetHero, owningCharacter, new ActionController.AttackProfile(1, 6, 0));

			foreach (Tile t in tilesAndDirection.tiles)
			{
				t.HideOverlay(Tile.OverlayType.PossibleAttck);
			}

			if (results.hit && targetHero.alive)
			{
				Util.PullToAttacker(targetHero, owningCharacter);
			}

			Finish();
		});
	}

	

	public static List<CardInstruction> GetCardInstructions(CardScriptableObject scriptableObject)
	{
		DisplayGrid.Instance.Clear(11, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction("Move towards closest enemy, stopping 3 away"));
		instructions.Add(new CardInstruction("Attack farthest enemy within 3 tiles for 1d6 damage"));
		instructions.Add(new CardInstruction("If target survives, pull them adjacent"));
		DisplayGrid.Instance.Show();

		return instructions;
	}
}
