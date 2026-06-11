using System;
using System.Collections.Generic;
using UnityEngine;

public class CrimsonClaw : Card
{
	TemplateLibrary.TilesAndDirection tilesAndDirection;
	Tuple<List<Tile>, Tile> route;

	public override void Execute()
	{
		Dictionary<Character, Tuple<List<Tile>, Tile>> routes = RouteToAllClosestCharacters(true);

		int maxRange = owningCharacter.characterDefinition.movement + 1;
		Dictionary<Character, Tuple<List<Tile>, Tile>> reachableRoutes = new Dictionary<Character, Tuple<List<Tile>, Tile>>(routes);
		Util.RemoveOutOfRangeRoutes(reachableRoutes, maxRange);

		route = null;
		foreach (KeyValuePair<Character, Tuple<List<Tile>, Tile>> pair in reachableRoutes)
		{
			if (route == null || pair.Value.Item1.Count > route.Item1.Count)
			{
				route = pair.Value;
			}
		}

		if (route == null)
		{
			route = Util.FindSmallestRoute(routes, null);
			if (route == null)
			{
				Finish();
				return;
			}
			Util.ShortenPathToMaxRange(route, maxRange);
		}

		AnimationController.Instance.ShowTiles(route.Item1, Tile.OverlayType.PossibleMovement, ReturnFromShowingTiles, ReturnFromRoute);
	}

	void ReturnFromShowingTiles()
	{
		TileGrid.Instance.RouteCharacterToTile(owningCharacter, new List<Tile>(route.Item1), ReturnFromRoute);
	}

	void ReturnFromRoute()
	{
		foreach (Tile t in route.Item1)
		{
			t.HideOverlay(Tile.OverlayType.PossibleMovement);
		}
		tilesAndDirection = TemplateLibrary.Instance.ChopTargeting(owningCharacter);
		if (tilesAndDirection == null)
		{
			Finish();
			return;
		}
		AnimationController.Instance.ShowTiles(tilesAndDirection.tiles, Tile.OverlayType.PossibleAttck, ReturnFromShowingAttackTiles);
	}

	void ReturnFromShowingAttackTiles()
	{
		owningCharacter.SetFacing(tilesAndDirection.direction);
		ActionController.Instance.PlayAttackAnimation(owningCharacter, null, () =>
		{
			foreach (Tile t in tilesAndDirection.tiles)
			{
				if (t.character != null && t.character.hero)
				{
					ActionController.AttackProfile profile = new ActionController.AttackProfile(1, 6, 0);
					profile.damageType = ActionPattern.DamageType.Burning;
					ActionController.Instance.AttackCharacter(t.character, owningCharacter, profile);
				}
				t.HideOverlay(Tile.OverlayType.PossibleAttck);
			}
			Finish();
		});
	}

	public static List<CardInstruction> GetCardInstructions(CardScriptableObject scriptableObject)
	{
		DisplayGrid.Instance.Clear(11, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction("Move to farthest reachable enemy"));
		instructions.Add(new CardInstruction("Hit enemies in chop pattern"));
		instructions.Add(new CardInstruction());
		instructions.Add(new CardInstruction("Deal 1d6 burning damage to each target"));
		DisplayGrid.Instance.Add(DisplayGrid.DisplayGridObject.Size1Enemy, DisplayGrid.DisplayGridDirection.South, 5, 4);
		DisplayGrid.Instance.Add(DisplayGrid.DisplayGridObject.EffectedTile, new List<Tuple<int, int>>()
		{
			new Tuple<int, int>(6, 3),
			new Tuple<int, int>(5, 3),
			new Tuple<int, int>(4, 3),
		});
		DisplayGrid.Instance.Show();
		return instructions;
	}
}
