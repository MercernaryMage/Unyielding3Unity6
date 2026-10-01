using System;
using System.Collections.Generic;
using UnityEngine;

public class ThornSpray : Card
{
	List<Tile> hitTiles;
	Tuple<List<Tile>, Tile> route;
	List<Tile> litRouteTiles;

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

		Tile currentTile = TileGrid.Instance.FindCharacter(owningCharacter)[0];
		Character closestHero = TileGrid.Instance.FindClosestHero(currentTile);
		if (closestHero != null)
		{
			Tile closestTile = TileGrid.Instance.GetClosestCharacterTile(currentTile, closestHero);
			owningCharacter.SetFacing(TileGrid.Instance.GetFacingDirection(currentTile, closestTile));
		}

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
					ActionController.Instance.AttackCharacter(t.character, owningCharacter, new ActionController.AttackProfile(1, GetIntValue("Damage"), 0));
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
		instructions.Add(new CardInstruction("Move to closest enemy"));
		instructions.Add(new CardInstruction("Hit enemies in pattern"));
		instructions.Add(new CardInstruction());
		instructions.Add(new CardInstruction($"Deal 1d{scriptableObject.GetTagIntValue("Damage")} damage to each target"));

		DisplayGrid.Instance.Add(DisplayGrid.DisplayGridObject.Size1Enemy, DisplayGrid.DisplayGridDirection.South, 5, 4);
		DisplayGrid.Instance.Add(DisplayGrid.DisplayGridObject.EffectedTile, new List<Tuple<int, int>>()
		{
			new Tuple<int, int>(4, 3),
			new Tuple<int, int>(5, 3),
			new Tuple<int, int>(6, 3),
			new Tuple<int, int>(4, 4),
			new Tuple<int, int>(6, 4),
			new Tuple<int, int>(4, 5),
			new Tuple<int, int>(5, 5),
			new Tuple<int, int>(6, 5),
		});
		DisplayGrid.Instance.Show();

		return instructions;
	}
}
