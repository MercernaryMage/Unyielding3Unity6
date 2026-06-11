using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Trample : Card
{
	TemplateLibrary.TilesAndDirection tilesAndDirection;
	int range = 4;
	List<Character> hitCharacters;

	public override void Execute()
	{
		tilesAndDirection = TemplateLibrary.Instance.GetMostCharactersInCardinalDirections(owningCharacter, range);

		if (!TileGrid.TilesContainHero(tilesAndDirection.tiles))
		{
			FloatingCombatNumberController.Instance.QueueFloatingCombatNumber(owningCharacter, "no target");
			Finish();
			return;
		}

		AnimationController.Instance.ShowTiles(tilesAndDirection.tiles, Tile.OverlayType.PossibleMovement, ReturnFromShowingTiles);
	}

	public void GoToFinish()
	{
		Finish();
	}

	public void ReturnFromShowingTiles()
	{
		Tuple<int, int> dir = TileGrid.directions[(int)tilesAndDirection.direction];
		List<Tile> characterOrigin = TileGrid.Instance.FindCharacter(owningCharacter);
		Tile startTile = characterOrigin[0];
		List<Tile> trail = GetTramplePath(owningCharacter, startTile, dir, range);
		while (trail.Count > 2)
		{
			trail.RemoveAt(1);
		}
		TileGrid.Instance.RouteCharacterToTile(owningCharacter, trail, ReturnFromRoute, false, false);
	}

	public void ReturnFromRoute()
	{
		DoTrample(owningCharacter, tilesAndDirection, range, GoToFinish);
	}

	public static List<Tile> GetTramplePath(Character character, Tile startTile, Tuple<int, int> dir, int range)
	{
		List<Tile> outTiles = new List<Tile>();
		for (int i = 1; i <= range; ++i)
		{
			Tile newTile = TileGrid.Instance.GetTile(startTile.x + dir.Item1 * i, startTile.y + dir.Item2 * i);
			if (newTile == null)
			{
				break;
			}
			if (TileGrid.Instance.WhatTilesWouldCharacterTake(character, newTile) == null)
			{
				break;
			}
			outTiles.Add(newTile);
		}
		return outTiles;
	}

	public static void DoTrample(Character character, TemplateLibrary.TilesAndDirection tilesAndDirection, int range, Action callback)
	{
		List<Tile> characterOrigin = TileGrid.Instance.FindCharacter(character);
		Tile startTile = characterOrigin[0];
		Tuple<int, int> dir = TileGrid.directions[(int)tilesAndDirection.direction];
		
		List<Tile> trail = GetTramplePath(character, startTile, dir, range);

		int i = trail.Count;
		Tile newPlacementTile = TileGrid.Instance.GetTile(startTile.x + dir.Item1 * i, startTile.y + dir.Item2 * i);
		List<Tile> impactTiles = TileGrid.Instance.WhatTilesWouldCharacterTake(character, newPlacementTile);
		List<Character> hitCharacters = new List<Character>();

		foreach (Tile t in tilesAndDirection.tiles)
		{
			t.HideOverlay(Tile.OverlayType.PossibleMovement);
		}

		List<Tile> actualSweptTiles = TemplateLibrary.Instance.GetTilesInMatchedSizeCardinalDirection(character, trail.Count, (int)tilesAndDirection.direction);
		foreach (Tile t in actualSweptTiles)
		{
			if (t.character != null)
			{
				if (t.character.hero)
				{
					hitCharacters.Add(t.character);
				}
				if (impactTiles.Contains(t))
				{
					ActionController.Instance.MoveCharacterFromImpact(t.character, impactTiles);
				}
			}
		}

		Direction direction = TileGrid.Instance.GetFacingDirection(startTile, newPlacementTile);
		TileGrid.Instance.MoveCharacterToTile(character, newPlacementTile);
		character.SetFacing(direction);
		hitCharacters = hitCharacters.Distinct().ToList();

		foreach (Character c in hitCharacters)
		{
			if (c.GetComponent<Stasis>() != null)
			{
				continue;
			}
			ActionController.AttackResults results = new ActionController.AttackResults();
			results.fakeHit = true;
			ActionController.Instance.DamageCharacter(c, character, new ActionController.AttackProfile(1, 6, 0), results);
			c.AddStatusEffect(typeof(Stun), null);
		}
		callback();
	}


	public static List<CardInstruction> GetCardInstructions(CardScriptableObject scriptableObject)
	{
		DisplayGrid.Instance.Clear(10, 10); 
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction("Select direction that would hit most enemies"));
		instructions.Add(new CardInstruction());
		instructions.Add(new CardInstruction("Move in that direction"));
		instructions.Add(new CardInstruction("Deal 1d6 damage to each target"));
		instructions.Add(new CardInstruction("Stun on hit"));
		
		DisplayGrid.Instance.Add(DisplayGrid.DisplayGridObject.Size2Enemy, DisplayGrid.DisplayGridDirection.South, 4, 4);
		DisplayGrid.Instance.Add(DisplayGrid.DisplayGridObject.Size2EnemyMove, DisplayGrid.DisplayGridDirection.South, 4, 0);
		DisplayGrid.Instance.Add(DisplayGrid.DisplayGridObject.Size2EnemyMove, DisplayGrid.DisplayGridDirection.North, 4, 8);
		DisplayGrid.Instance.Add(DisplayGrid.DisplayGridObject.Size2EnemyMove, DisplayGrid.DisplayGridDirection.West, 0, 4);
		DisplayGrid.Instance.Add(DisplayGrid.DisplayGridObject.Size2EnemyMove, DisplayGrid.DisplayGridDirection.East, 8, 4);
		DisplayGrid.Instance.Add(DisplayGrid.DisplayGridObject.EffectedTile, new List<Tuple<int, int>>()
		{
			new Tuple<int, int>(4,6),
			new Tuple<int, int>(5,6),
			new Tuple<int, int>(4,7),
			new Tuple<int, int>(5,7),

			new Tuple<int, int>(4,2),
			new Tuple<int, int>(5,2),
			new Tuple<int, int>(4,3),
			new Tuple<int, int>(5,3),

			new Tuple<int, int>(2,5),
			new Tuple<int, int>(3,5),
			new Tuple<int, int>(2,4),
			new Tuple<int, int>(3,4),

			new Tuple<int, int>(6,5),
			new Tuple<int, int>(7,5),
			new Tuple<int, int>(6,4),
			new Tuple<int, int>(7,4),
		});
		DisplayGrid.Instance.Show();




		return instructions;
	}
}
