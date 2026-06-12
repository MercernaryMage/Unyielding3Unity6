using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BloodThirstingFangs : Card
{
	TemplateLibrary.TilesAndDirection tilesAndDirection;
	List<Character> hitCharacters;
	Tuple<List<Tile>, Tile> route;
	Tile startTile;
	Character lowestHPHero;

	//Find closest enemy

	public override void Execute()
	{
		Dictionary<Character, Tuple<List<Tile>, Tile>> routes = RouteToAllClosestCharacters(true);

		if (routes.Count == 0)
		{
			Debug.Log("No possible route");
			Finish();
			return;
		}

		lowestHPHero = null;
		foreach (KeyValuePair<Character, Tuple<List<Tile>, Tile>> pairs in routes)
		{
			if (lowestHPHero == null)
			{
				lowestHPHero = pairs.Key;
			}
			else
			{
				if (lowestHPHero.currentHP > pairs.Key.currentHP)
				{
					lowestHPHero = pairs.Key;
				}
			}
		}

		route = routes[lowestHPHero];
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
		foreach (Tile t in route.Item1)
		{
			t.HideOverlay(Tile.OverlayType.PossibleMovement);
		}

		if (!TileGrid.AreCharactersAdjacent(lowestHPHero, owningCharacter))
		{
			Debug.Log("Not in range");
			Finish();
			return;
		}

		List<Tile> tiles = TileGrid.Instance.FindCharacter(lowestHPHero);
		if (tiles == null)
		{
			Debug.Log("No targets");
			Finish();
			return;
		}
		tilesAndDirection = new TemplateLibrary.TilesAndDirection(tiles, Direction.East);
		AnimationController.Instance.ShowTiles(tiles, Tile.OverlayType.PossibleAttck, ReturnFromShowingAttackTiles);
	}

	List<Tile> GetAdjacentCharacterTarget()
	{
		List<Tile> tiles = TileGrid.Instance.GetAllAdjacentTilesToCharacter(owningCharacter);
		List<Character> targets = new List<Character>();
		foreach (Tile tile in tiles)
		{
			if (tile.character && tile.character.hero && tile.character.GetComponent<Slobbered>())
			{
				if (!targets.Contains(tile.character))
				{
					targets.Add(tile.character);
				}
			}
		}
		if (targets.Count > 0)
		{
			return TileGrid.Instance.FindCharacter(targets[0]);
		}
		foreach (Tile tile in tiles)
		{
			if (tile.character && !tile.character.hero)
			{
				if (!targets.Contains(tile.character))
				{
					targets.Add(tile.character);
				}
			}
		}
		if (targets.Count > 0)
		{
			return TileGrid.Instance.FindCharacter(targets[0]);
		}
		return null;
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
		instructions.Add(new CardInstruction("Move to lowest hp enemy"));
		instructions.Add(new CardInstruction("Attack enemy for 1d6 damage"));

		DisplayGrid.Instance.Show();

		return instructions;
	}
}
