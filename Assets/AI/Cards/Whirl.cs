using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Whirl : Card
{
	List<Tile> hitTiles;
	List<Character> hitCharacters;
	Tuple<List<Tile>, Tile> route;

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

		AnimationController.Instance.ShowTiles(route.Item1, Tile.OverlayType.PossibleMovement, Route, ReturnFromShowingTiles);
	}

	public void Route()
	{
		TileGrid.Instance.RouteCharacterToTile(owningCharacter, new List<Tile>(route.Item1), ReturnFromShowingTiles);
	}

	public void ReturnFromShowingTiles()
	{
		List<Tile> characterOrigin = TileGrid.Instance.FindCharacter(owningCharacter);
		TileGrid.Instance.MoveCharacterToTile(owningCharacter, route.Item1[route.Item1.Count - 1]);

		Tile currentTile = route.Item1[route.Item1.Count - 1];
		Character closestHero = TileGrid.Instance.FindClosestHero(currentTile);

		if (closestHero != null)
		{
			Tile closestTile = TileGrid.Instance.GetClosestCharacterTile(currentTile, closestHero);
			Direction direction = TileGrid.Instance.GetFacingDirection(currentTile, closestTile);
			owningCharacter.SetFacing(direction);
		}
		else
		{
			Direction direction = TileGrid.Instance.GetFacingDirection(characterOrigin[0], route.Item2);
			owningCharacter.SetFacing(direction);
		}

		foreach (Tile t in route.Item1)
		{
			t.HideOverlay(Tile.OverlayType.PossibleMovement);
		}
		DoTargeting();
	}

	public void DoTargeting()
	{
		hitTiles = TemplateLibrary.Instance.GetCloseAoE(owningCharacter);
		if (hitTiles == null)
		{
			Debug.Log("No targets in range");
			Finish();
			return;
		}
		AnimationController.Instance.ShowTiles(hitTiles, Tile.OverlayType.PossibleAttck, ReturnFromShowingAttackTiles);
	}

	public void ReturnFromShowingAttackTiles()
	{
		List<Character> hitCharacters = new List<Character>();
		ActionController.Instance.PlayAttackAnimation(owningCharacter, null, () =>
		{
			foreach (Tile t in hitTiles)
			{
				if (t.character && t.character.hero && !hitCharacters.Contains(t.character))
				{
					hitCharacters.Add(t.character);
					ActionController.Instance.AttackCharacter(t.character, owningCharacter, new ActionController.AttackProfile(1, 6, 0));
					ActionController.Instance.KnockBack(t.character, TileGrid.Instance.FindCharacter(owningCharacter), 1);
				}
				t.HideOverlay(Tile.OverlayType.PossibleAttck);
			}

			Finish();
		});
	}

	public static List<CardInstruction> GetCardInstructions(CardScriptableObject scriptableObject)
	{
		DisplayGrid.Instance.Clear(12, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction("Move to closest enemy"));
		instructions.Add(new CardInstruction("Hit enemies in pattern"));
		instructions.Add(new CardInstruction());
		instructions.Add(new CardInstruction("Deal 1d6 damage to each target"));
		instructions.Add(new CardInstruction("Knockback 1 on hit"));

		DisplayGrid.Instance.Add(DisplayGrid.DisplayGridObject.Size2Enemy, DisplayGrid.DisplayGridDirection.South, 5, 4);
		DisplayGrid.Instance.Add(DisplayGrid.DisplayGridObject.EffectedTile, new List<Tuple<int, int>>()
		{
			new Tuple<int, int>(4, 4),
			new Tuple<int, int>(4, 5),
			new Tuple<int, int>(4, 6),
			new Tuple<int, int>(5, 6),
			new Tuple<int, int>(6, 6),
			new Tuple<int, int>(7, 6),
			new Tuple<int, int>(7, 5),
			new Tuple<int, int>(7, 4),
			new Tuple<int, int>(7, 3),
			new Tuple<int, int>(6, 3),
			new Tuple<int, int>(5, 3),
			new Tuple<int, int>(4, 3),
		});
		DisplayGrid.Instance.Add(DisplayGrid.DisplayGridObject.KnockbackArrow, DisplayGrid.DisplayGridDirection.West, 4, 4);
		DisplayGrid.Instance.Add(DisplayGrid.DisplayGridObject.KnockbackArrow, DisplayGrid.DisplayGridDirection.East, 7, 4);
		DisplayGrid.Instance.Add(DisplayGrid.DisplayGridObject.KnockbackArrow, DisplayGrid.DisplayGridDirection.North, 5, 6);
		DisplayGrid.Instance.Add(DisplayGrid.DisplayGridObject.KnockbackArrow, DisplayGrid.DisplayGridDirection.South, 5, 3);
		DisplayGrid.Instance.Show();




		return instructions;
	}
}
