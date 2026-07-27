using System;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class CrimsonClaw : Card
{
	TemplateLibrary.TilesAndDirection tilesAndDirection;
	Tuple<List<Tile>, Tile> route;
	List<Tile> litRouteTiles;
	Character target;

	public override void Execute()
	{
		Dictionary<Character, Tuple<List<Tile>, Tile>> routes = RouteToAllClosestCharacters(true);

		int maxRange = owningCharacter.characterDefinition.movement + 1;
		Dictionary<Character, Tuple<List<Tile>, Tile>> reachableRoutes = new Dictionary<Character, Tuple<List<Tile>, Tile>>(routes);
		Util.RemoveOutOfRangeRoutes(reachableRoutes, maxRange);

		route = null;
		target = null;
		foreach (KeyValuePair<Character, Tuple<List<Tile>, Tile>> pair in reachableRoutes)
		{
			if (route == null || pair.Value.Item1.Count > route.Item1.Count)
			{
				route = pair.Value;
				target = pair.Key;
			}
		}

		if (route == null)
		{
			KeyValuePair<Character, Tuple<List<Tile>, Tile>> closest = Util.FindSmallestRoutePair(routes, null);
			target = closest.Key;
			route = closest.Value;
			if (route == null)
			{
				Finish();
				return;
			}
			Util.ShortenPathToMaxRange(route, maxRange);
		}

		litRouteTiles = Util.ExpandPathTiles(route.Item1, owningCharacter);
		AnimationController.Instance.ShowTiles(litRouteTiles, Tile.OverlayType.PossibleMovement, ReturnFromShowingTiles, ReturnFromRoute);
	}

	void ReturnFromShowingTiles()
	{
		TileGrid.Instance.RouteAICharacterToTile(owningCharacter, new List<Tile>(route.Item1), ReturnFromRoute);
	}

	void ReturnFromRoute()
	{
		foreach (Tile t in litRouteTiles)
		{
			t.HideOverlay(Tile.OverlayType.PossibleMovement);
		}
		tilesAndDirection = null;
		if (target != null && target.alive && !target.IsDowned())
		{
			tilesAndDirection = TemplateLibrary.Instance.ChopTargetingTowardCharacter(owningCharacter, target);
		}
		//If we couldn't line up a chop on the main target, attack whoever we can reach instead.
		if (tilesAndDirection == null)
		{
			tilesAndDirection = TemplateLibrary.Instance.ChopTargeting(owningCharacter);
		}
		if (tilesAndDirection == null)
		{
			Finish();
			return;
		}
		AnimationController.Instance.ShowTiles(tilesAndDirection.tiles, Tile.OverlayType.PossibleAttck, ReturnFromShowingAttackTiles);
	}

	void ReturnFromShowingAttackTiles()
	{
		owningCharacter.SetFacing(tilesAndDirection.direction);
		List<Character> targets = new List<Character>();
		foreach (Tile t in tilesAndDirection.tiles)
		{
			if (t.character != null && t.character.hero)
			{
				targets.Add(t.character);
			}
		}
		ActionController.Instance.PlayAdvancedAttackAnimation(owningCharacter, targets, cardScriptableObject.effects[0], null, () =>
		{
			foreach (Character t in targets)
			{
				ActionController.AttackProfile profile = new ActionController.AttackProfile(1, 6, 0);
				profile.damageType = ActionPattern.DamageType.Burning;
				ActionController.Instance.AttackCharacter(t, owningCharacter, profile);
			}
			foreach (Tile t in tilesAndDirection.tiles)
			{
				t.HideOverlay(Tile.OverlayType.PossibleAttck);
			}
			Finish();
		});
	}

	public static List<CardInstruction> GetCardInstructions(CardScriptableObject scriptableObject)
	{
		DisplayGrid.Instance.Clear(11, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction("Move to farthest reachable enemy"));
		instructions.Add(new CardInstruction("Hit enemies in chop pattern"));
		instructions.Add(new CardInstruction());
		instructions.Add(new CardInstruction("Deal 1d6 <u>Burning</u> damage to each target"));
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
