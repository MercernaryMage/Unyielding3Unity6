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

		AnimationController.Instance.ShowTiles(route.Item1, Tile.OverlayType.PossibleMovement, Route, ReturnFromRoute);
	}

	public void Route()
	{
		TileGrid.Instance.RouteAICharacterToTile(owningCharacter, new List<Tile>(route.Item1), ReturnFromRoute);
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
				if (t.character && t.character.hero)
				{

					ActionController.AttackResults results = ActionController.Instance.AttackCharacter(t.character, owningCharacter, new ActionController.AttackProfile(1, 6, 0));
					if (results.damageDealt >= 3)
					{
						HowlAndGrowl.Howl(owningCharacter, 1);
					}
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
		instructions.Add(new CardInstruction("Move to closest enemy"));
		instructions.Add(new CardInstruction("Attack character for 1d6 damage"));
		instructions.Add(new CardInstruction("If 3 or more damage was dealt, Howl"));
		DisplayGrid.Instance.Show();




		return instructions;
	}
}
