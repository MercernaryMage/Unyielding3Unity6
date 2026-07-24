using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Charge : Card
{
	Tuple<List<Tile>, Tile> route;
	List<Tile> litRouteTiles;
	TemplateLibrary.TilesAndDirection tilesAndDirection;
	HashSet<Character> pushedCharacters = new HashSet<Character>();

	public override void Execute()
	{
		pushedCharacters.Clear();
		Dictionary<Character, Tuple<List<Tile>, Tile>> routes = RouteToAllClosestCharacters(true);
		route = Util.FindSmallestRoute(routes, null);

		if (route == null)
		{
			Finish();
			return;
		}

		Util.ShortenPathToMaxRange(route, owningCharacter.characterDefinition.movement + 1);
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

		// Prefer charging the way we face, but turn toward another direction with a hero ahead
		List<Direction> directionsToTry = new List<Direction>() { owningCharacter.facing };
		for (int i = 0; i < TileGrid.directions.Count; ++i)
		{
			if ((Direction)i != owningCharacter.facing)
			{
				directionsToTry.Add((Direction)i);
			}
		}
		foreach (Direction direction in directionsToTry)
		{
			if (GetHeroesDirectlyAhead(direction).Count > 0)
			{
				if (direction != owningCharacter.facing)
				{
					owningCharacter.SetFacing(direction);
					AnimationController.Instance.DelayedCallback(0.5f, () => DoPushStep(0));
					return;
				}
				break;
			}
		}

		DoPushStep(0);
	}

	HashSet<Character> GetHeroesDirectlyAhead(Direction direction)
	{
		Tuple<int, int> dir = TileGrid.directions[(int)direction];
		List<Tile> chargerTiles = TileGrid.Instance.FindCharacter(owningCharacter);
		HashSet<Character> heroes = new HashSet<Character>();
		foreach (Tile ct in chargerTiles)
		{
			Tile ahead = TileGrid.Instance.GetTile(ct.x + dir.Item1, ct.y + dir.Item2);
			if (ahead == null || chargerTiles.Contains(ahead))
			{
				continue;
			}
			if (ahead.character != null && ahead.character.hero && ahead.character.GetComponent<Downed>() == null)
			{
				heroes.Add(ahead.character);
			}
		}
		return heroes;
	}

	void DoPushStep(int step)
	{
		if (step >= 4)
		{
			AttackAfterPush();
			return;
		}

		Tuple<int, int> dir = TileGrid.directions[(int)owningCharacter.facing];
		int dx = dir.Item1;
		int dy = dir.Item2;

		List<Tile> chargerTiles = TileGrid.Instance.FindCharacter(owningCharacter);
		HashSet<Character> toPush = GetHeroesDirectlyAhead(owningCharacter.facing);

		if (toPush.Count == 0)
		{
			AttackAfterPush();
			return;
		}

		// Any hero immediately ahead of a member in the push direction also joins the chain
		bool changed = true;
		while (changed)
		{
			changed = false;
			foreach (Character c in toPush.ToList())
			{
				foreach (Tile ct in TileGrid.Instance.FindCharacter(c))
				{
					Tile ahead = TileGrid.Instance.GetTile(ct.x + dx, ct.y + dy);
					if (ahead == null || ahead.character == null || ahead.character == c)
					{
						continue;
					}
					if (ahead.character.hero && ahead.character.GetComponent<Downed>() == null && !toPush.Contains(ahead.character))
					{
						toPush.Add(ahead.character);
						changed = true;
					}
				}
			}
		}

		// Stop if any character would be pushed into a wall or an ally of the charger
		bool blocked = false;
		foreach (Character c in toPush)
		{
			foreach (Tile ct in TileGrid.Instance.FindCharacter(c))
			{
				Tile dest = TileGrid.Instance.GetTile(ct.x + dx, ct.y + dy);
				if (dest == null || !dest.tileScriptableObject.enterable)
				{
					blocked = true;
					break;
				}
				if (dest.character != null && dest.character != c && !toPush.Contains(dest.character))
				{
					blocked = true;
					break;
				}
			}
			if (blocked)
			{
				break;
			}
		}

		// Stop if the charger itself cannot step forward
		if (!blocked)
		{
			Tile newChargerAnchor = TileGrid.Instance.GetTile(chargerTiles[0].x + dx, chargerTiles[0].y + dy);
			List<Tile> newChargerFootprint = newChargerAnchor != null
				? TileGrid.Instance.WhatTilesWouldCharacterTake(owningCharacter, newChargerAnchor)
				: null;
			if (newChargerFootprint == null)
			{
				blocked = true;
			}
			else
			{
				foreach (Tile ft in newChargerFootprint)
				{
					if (ft.character != null && ft.character != owningCharacter && !toPush.Contains(ft.character))
					{
						blocked = true;
						break;
					}
				}
			}
		}

		if (blocked)
		{
			AttackAfterPush();
			return;
		}

		// Push furthest-in-direction first to avoid collisions, then advance charger
		List<Character> ordered = toPush
			.OrderByDescending(c => TileGrid.Instance.FindCharacter(c).Max(t => t.x * dx + t.y * dy))
			.ToList();

		foreach (Character c in ordered)
		{
			Tile anchor = TileGrid.Instance.FindCharacter(c)[0];
			TileGrid.Instance.MoveCharacterToTile(c, TileGrid.Instance.GetTile(anchor.x + dx, anchor.y + dy));
			pushedCharacters.Add(c);
		}

		TileGrid.Instance.MoveCharacterToTile(owningCharacter, TileGrid.Instance.GetTile(chargerTiles[0].x + dx, chargerTiles[0].y + dy));

		AnimationController.Instance.DelayedCallback(0.5f, () => DoPushStep(step + 1));
	}

	void AttackAfterPush()
	{
		List<Character> pushed = new List<Character>(pushedCharacters);
		foreach (Character c in pushedCharacters)
		{
			CharacterKnockbackFinishMessage characterKnockbackFinishMessage = new CharacterKnockbackFinishMessage();
			characterKnockbackFinishMessage.knockedBackCharacter = c;
			MessagePump.Instance.SendMessage(characterKnockbackFinishMessage);
		}
		pushedCharacters.Clear();

		tilesAndDirection = null;
		foreach (Character c in pushed)
		{
			if (!c.alive)
			{
				continue;
			}
			TemplateLibrary.TilesAndDirection candidate = TemplateLibrary.Instance.ChopTargetingTowardCharacter(owningCharacter, c);
			if (candidate != null && TemplateHitsCharacter(candidate, c))
			{
				tilesAndDirection = candidate;
				break;
			}
		}

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

	bool TemplateHitsCharacter(TemplateLibrary.TilesAndDirection template, Character c)
	{
		foreach (Tile t in template.tiles)
		{
			if (t.character == c)
			{
				return true;
			}
		}
		return false;
	}

	void ReturnFromShowingAttackTiles()
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
		instructions.Add(new CardInstruction("Move to closest enemy"));
		instructions.Add(new CardInstruction("Charge forward 4 spaces, pushing all enemies"));
		DisplayGrid.Instance.Show();
		return instructions;
	}
}
