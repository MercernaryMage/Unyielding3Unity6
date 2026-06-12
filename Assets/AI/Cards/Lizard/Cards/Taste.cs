using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Taste : Card
{
	TemplateLibrary.TilesAndDirection tilesAndDirection;
	List<Character> hitCharacters;
	Tuple<List<Tile>, Tile> route;
	Tile startTile;

	//Find closest enemy


	public bool IsSlobbered(Character hero)
	{
		return hero.GetComponent<Slobbered>() != null;
	}

	public override void Execute()
	{
		List<Tile> sideTiles = TileGrid.Instance.GetSideTiles(owningCharacter);

		foreach (Tile t in sideTiles)
		{
			if (t.character != null && t.character.hero == true)
			{
				t.character.AddStatusEffect(typeof(Slobbered), null);
			}
		}

		Dictionary<Character, Tuple<List<Tile>, Tile>> routes = RouteToAllClosestCharacters(true);

		route = Util.FindSmallestRoute(routes, IsSlobbered);
		if (route == null)
		{
			FloatingCombatNumberController.Instance.QueueFloatingCombatNumber(owningCharacter, "no target");
			Finish();
			return;
		}

		Util.ShortenPathToMaxRange(route, owningCharacter.characterDefinition.movement + 1);

		startTile = TileGrid.Instance.FindCharacter(owningCharacter)[0];

		AnimationController.Instance.ShowTiles(route.Item1, Tile.OverlayType.PossibleMovement, Route, ReturnFromRoute);
	}

	bool TargetSlobbered(Character c)
	{
		return c.GetComponent<Slobbered>() != null;
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
		List<Tile> tiles = TemplateLibrary.GetAdjacentCharacterTarget(owningCharacter, TargetSlobbered);
		if (tiles == null)
		{
			tiles = TemplateLibrary.GetAdjacentCharacterTarget(owningCharacter, null);
			if (tiles == null)
			{
				Debug.Log("No targets");
				Finish();
				return;
			}
		}
		tilesAndDirection = new TemplateLibrary.TilesAndDirection(tiles, Direction.East);
		AnimationController.Instance.ShowTiles(tiles, Tile.OverlayType.PossibleAttck, ReturnFromShowingAttackTiles);
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
		instructions.Add(new CardInstruction("Slobber all adjacent enemies to the side"));
		instructions.Add(new CardInstruction("Move to closest slobbered enemy"));
		instructions.Add(new CardInstruction());
		instructions.Add(new CardInstruction("Attack enemy for 1d6 damage"));




		return instructions;
	}
}
