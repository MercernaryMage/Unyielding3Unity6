using System;
using System.Collections.Generic;
using UnityEngine;

public class BigBite : Card
{
	TemplateLibrary.TilesAndDirection tilesAndDirection;
	Tuple<List<Tile>, Tile> route;
	List<Tile> litRouteTiles;
	Character target;

	bool IsParalyzed(Character c)
	{
		if (c.GetComponent<Downed>())
		{
			return false;
		}
		return c.GetComponent<Paralyzed>() != null;
	}

	public override void Execute()
	{
		Dictionary<Character, Tuple<List<Tile>, Tile>> routes = RouteToAllClosestCharacters(true);

		KeyValuePair<Character, Tuple<List<Tile>, Tile>> closest = Util.FindSmallestRoutePair(routes, IsParalyzed);
		if (closest.Key == null)
		{
			closest = Util.FindSmallestRoutePair(routes, null);
		}

		target = closest.Key;
		route = closest.Value;
		if (route == null)
		{
			Debug.Log("No possible route");
			Finish();
			return;
		}

		Util.ShortenPathToMaxRange(route, owningCharacter.characterDefinition.movement + 1);

		litRouteTiles = Util.ExpandPathTiles(route.Item1, owningCharacter);
		AnimationController.Instance.ShowTiles(litRouteTiles, Tile.OverlayType.PossibleMovement, ReturnFromShowingTiles, ReturnFromRoute);
	}

	public void ReturnFromShowingTiles()
	{
		TileGrid.Instance.RouteAICharacterToTile(owningCharacter, new List<Tile>(route.Item1), ReturnFromRoute);
	}

	public void ReturnFromRoute()
	{
		foreach (Tile t in litRouteTiles)
		{
			t.HideOverlay(Tile.OverlayType.PossibleMovement);
		}

		tilesAndDirection = null;
		if (target != null && target.alive)
		{
			tilesAndDirection = TemplateLibrary.Instance.SizeSquareTargetingTowardCharacter(owningCharacter, target);
		}
		if (tilesAndDirection == null)
		{
			tilesAndDirection = TemplateLibrary.Instance.SizeSquareTargeting(owningCharacter);
		}
		if (tilesAndDirection == null)
		{
			FloatingCombatNumberController.Instance.QueueFloatingCombatNumber(owningCharacter, "No target");
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

	public static List<CardInstruction> GetCardInstructions(CardScriptableObject scriptableObject)
	{
		DisplayGrid.Instance.Clear(11, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction("Move to the closest <u>Paralyzed</u> enemy"));
		instructions.Add(new CardInstruction("or the closest enemy if none are <u>Paralyzed</u>"));
		instructions.Add(new CardInstruction("Bite a square as wide as this character"));
		instructions.Add(new CardInstruction("Deal 1d6 damage to each enemy in it"));

		DisplayGrid.Instance.Add(DisplayGrid.DisplayGridObject.Size1Enemy, DisplayGrid.DisplayGridDirection.South, 5, 4);
		DisplayGrid.Instance.Add(DisplayGrid.DisplayGridObject.EffectedTile, new List<Tuple<int, int>>()
		{
			new Tuple<int, int>(5, 3),
		});
		DisplayGrid.Instance.Show();

		return instructions;
	}
}
