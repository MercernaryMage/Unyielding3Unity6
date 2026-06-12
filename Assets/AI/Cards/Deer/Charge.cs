using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Charge : Card
{
	Tuple<List<Tile>, Tile> route;
	TemplateLibrary.TilesAndDirection tilesAndDirection;

	public override void Execute()
	{
		Dictionary<Character, Tuple<List<Tile>, Tile>> routes = RouteToAllClosestCharacters(true);
		route = Util.FindSmallestRoute(routes, null);

		if (route == null)
		{
			Finish();
			return;
		}

		Util.ShortenPathToMaxRange(route, owningCharacter.characterDefinition.movement + 1);
		AnimationController.Instance.ShowTiles(route.Item1, Tile.OverlayType.PossibleMovement, ReturnFromShowingTiles, ReturnFromRoute);
	}

	void ReturnFromShowingTiles()
	{
		TileGrid.Instance.RouteAICharacterToTile(owningCharacter, new List<Tile>(route.Item1), ReturnFromRoute);
	}

	void ReturnFromRoute()
	{
		foreach (Tile t in route.Item1)
		{
			t.HideOverlay(Tile.OverlayType.PossibleMovement);
		}

		DoPushStep(0);
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
		HashSet<Character> toPush = new HashSet<Character>();
		foreach (Tile ct in chargerTiles)
		{
			Tile ahead = TileGrid.Instance.GetTile(ct.x + dx, ct.y + dy);
			if (ahead == null || chargerTiles.Contains(ahead))
			{
				continue;
			}
			if (ahead.character != null && ahead.character.hero && ahead.character.GetComponent<Downed>() == null)
			{
				toPush.Add(ahead.character);
			}
		}

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
		}

		TileGrid.Instance.MoveCharacterToTile(owningCharacter, TileGrid.Instance.GetTile(chargerTiles[0].x + dx, chargerTiles[0].y + dy));

		AnimationController.Instance.DelayedCallback(0.5f, () => DoPushStep(step + 1));
	}

	void AttackAfterPush()
	{
		tilesAndDirection = TemplateLibrary.Instance.ChopTargeting(owningCharacter);
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
