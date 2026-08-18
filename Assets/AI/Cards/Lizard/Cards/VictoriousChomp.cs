using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VictoriousChomp : Card
{
	TemplateLibrary.TilesAndDirection tilesAndDirection;
	List<Character> hitCharacters;
	Tuple<List<Tile>, Tile> route;
	List<Tile> litRouteTiles;
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
			int howlCount = 0;
			foreach (Tile t in tilesAndDirection.tiles)
			{
				if (t.character && t.character.hero)
				{

					ActionController.AttackResults results = ActionController.Instance.AttackCharacter(t.character, owningCharacter, new ActionController.AttackProfile(1, 6, 0));
					if (results.damageDealt >= 3)
					{
						++howlCount;
					}
					t.HideOverlay(Tile.OverlayType.PossibleAttck);
				}
			}

			if (howlCount == 0)
			{
				Finish();
				return;
			}
			HowlAndGrowl.Howl(owningCharacter, howlCount, Finish);
		});
	}

	public static List<CardInstruction> GetCardInstructions(CardScriptableObject scriptableObject)
	{
		DisplayGrid.Instance.Clear(11, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction("Move to closest enemy"));
		instructions.Add(new CardInstruction("Attack character for 1d6 damage"));
		instructions.Add(new CardInstruction("If 3 or more damage was dealt, <u>Howl</u>"));
		DisplayGrid.Instance.Show();




		return instructions;
	}
}
