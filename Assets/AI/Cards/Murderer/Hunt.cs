using System;
using System.Collections.Generic;
using UnityEngine;

public class Hunt : Card
{
	TemplateLibrary.TilesAndDirection tilesAndDirection;
	Tuple<List<Tile>, Tile> route;
	List<Tile> litRouteTiles;

	const int isolatedMovement = 5;
	const int closestMovement = 3;

	public bool IsIsolated(Character target)
	{
		if (target.GetComponent<Recovering>())
		{
			return false;
		}
		return TileGrid.Instance.IsTargetIsolated(target, owningCharacter);
	}

	public override void Execute()
	{
		Dictionary<Character, Tuple<List<Tile>, Tile>> routes = RouteToAllClosestCharacters(true);

		int maxRange = isolatedMovement + 1;

		Dictionary<Character, Tuple<List<Tile>, Tile>> isolatedRoutes = new Dictionary<Character, Tuple<List<Tile>, Tile>>(routes);
		Util.RemoveOutOfRangeRoutes(isolatedRoutes, maxRange);
		route = Util.FindSmallestRoute(isolatedRoutes, IsIsolated);

		if (route == null)
		{
			route = Util.FindSmallestRoute(routes, null);
			maxRange = closestMovement + 1;
		}

		if (route == null)
		{
			Debug.Log("No possible route");
			Finish();
			return;
		}

		Util.ShortenPathToMaxRange(route, maxRange);

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

		List<Tile> tiles = TemplateLibrary.GetAdjacentCharacterTarget(owningCharacter, null);
		if (tiles == null)
		{
			Debug.Log("No targets");
			Finish();
			return;
		}
		tilesAndDirection = new TemplateLibrary.TilesAndDirection(tiles, Direction.East);

		AnimationController.Instance.ShowTiles(tilesAndDirection.tiles, Tile.OverlayType.PossibleAttck, ReturnFromShowingAttackTiles);
	}

	public void ReturnFromShowingAttackTiles()
	{
		owningCharacter.SetFacing(TileGrid.Instance.GetFacingDirection(owningCharacter, tilesAndDirection.tiles[0].character));
		ActionController.Instance.PlayAttackAnimation(owningCharacter, null, () =>
		{
			foreach (Tile t in tilesAndDirection.tiles)
			{
				if (t.character != null && t.character.hero)
				{
					ActionController.Instance.AttackCharacter(t.character, owningCharacter, new ActionController.AttackProfile(1, 6, 0));
					t.HideOverlay(Tile.OverlayType.PossibleAttck);
				}
			}

			Finish();
		});
	}

	public static List<CardInstruction> GetCardInstructions(CardScriptableObject scriptableObject, Character character )
	{
		DisplayGrid.Instance.Clear(11, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction($"Move up to {isolatedMovement} to the closest isolated enemy in range {isolatedMovement}"));
		instructions.Add(new CardInstruction($"or move up to {closestMovement} toward the closest enemy"));
		instructions.Add(new CardInstruction("Attack adjacent enemy for 1d6 damage"));
		DisplayGrid.Instance.Show();

		return instructions;
	}
}
