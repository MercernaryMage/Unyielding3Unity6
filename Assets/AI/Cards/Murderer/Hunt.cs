using System;
using System.Collections.Generic;
using UnityEngine;

public class Hunt : Card
{
	TemplateLibrary.TilesAndDirection tilesAndDirection;
	Tuple<List<Tile>, Tile> route;

	bool IsIsolated(Character target)
	{
		if (target.GetComponent<Downed>())
		{
			return false;
		}
		return TileGrid.Instance.IsTargetIsolated(target, owningCharacter);
	}

	public override void Execute()
	{
		Dictionary<Character, Tuple<List<Tile>, Tile>> routes = RouteToAllClosestCharacters(true);

		route = Util.FindSmallestRoute(routes, IsIsolated);
		if (route == null)
		{
			route = Util.FindSmallestRoute(routes, null);
		}

		if (route == null)
		{
			Debug.Log("No possible route");
			Finish();
			return;
		}

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

	public static List<CardInstruction> GetCardInstructions(CardScriptableObject scriptableObject)
	{
		DisplayGrid.Instance.Clear(11, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction("Move to closest isolated enemy"));
		instructions.Add(new CardInstruction("or closest enemy if none isolated"));
		instructions.Add(new CardInstruction("Attack adjacent enemy for 1d6 damage"));
		DisplayGrid.Instance.Show();

		return instructions;
	}
}
