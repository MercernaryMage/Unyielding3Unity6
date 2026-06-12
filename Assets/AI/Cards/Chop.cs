using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Chop : Card
{
	TemplateLibrary.TilesAndDirection tilesAndDirection;
	List<Character> hitCharacters;
	Tuple<List<Tile>, Tile> route;
	Tile startTile;

	//Find closest enemy

	public override void Execute()
	{
		Dictionary<Character, Tuple<List<Tile>, Tile>> routes = RouteToAllClosestCharacters(true);

		route = Util.FindSmallestRoute(routes, null);
		if (route == null)
		{
			Debug.Log("No possible route");
			Finish();
			return;
		}

		Util.ShortenPathToMaxRange(route, owningCharacter.characterDefinition.movement + 1);

		startTile = TileGrid.Instance.FindCharacter(owningCharacter)[0];

		AnimationController.Instance.ShowTiles(route.Item1, Tile.OverlayType.PossibleMovement, ReturnFromShowingTiles, ReturnFromRoute);
	}

	public void ReturnFromShowingTiles()
	{
		TileGrid.Instance.RouteAICharacterToTile(owningCharacter, new List<Tile>(route.Item1), ReturnFromRoute);
	}

	public void ReturnFromRoute()
	{
		//Direction direction = TileGrid.Instance.GetFacingDirection(startTile, TileGrid.Instance.FindCharacter(owningCharacter)[0]);
		//owningCharacter.SetFacing(direction);
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

	public void ReturnFromShowingAttackTiles()
	{
		owningCharacter.SetFacing(tilesAndDirection.direction);
		ActionController.Instance.PlayAttackAnimation(owningCharacter, null, () =>
		{
			foreach (Tile t in tilesAndDirection.tiles)
			{
				if (t.character && t.character.hero)
				{
					ActionController.Instance.AttackCharacter(t.character, owningCharacter, new ActionController.AttackProfile(1, 6, 0));
				}
				t.HideOverlay(Tile.OverlayType.PossibleAttck);
			}

			Finish();
		});
		
	}

	public static List<CardInstruction> GetCardInstructions2()
	{
		return null;
	}

	public static List<CardInstruction> GetCardInstructions(CardScriptableObject scriptableObject)
	{
		DisplayGrid.Instance.Clear(11, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction("Move to closest enemy"));
		instructions.Add(new CardInstruction("Hit enemies in pattern"));
		instructions.Add(new CardInstruction());
		instructions.Add(new CardInstruction("Deal 1d6 damage to each target"));

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
