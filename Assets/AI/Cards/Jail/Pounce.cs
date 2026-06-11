using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pounce
{
	TemplateLibrary.TilesAndDirection tilesAndDirection;
	int range = 2;
	List<Character> hitCharacters;
	Character owningCharacter;
	Action callback;

	public void Execute(Character o, Character targetCharacter, Action actionToStore)
	{
		callback = actionToStore;
		owningCharacter = o;
		tilesAndDirection = TemplateLibrary.Instance.GetPounceTemplate(owningCharacter, targetCharacter, range);

// 		if (!TileGrid.TilesContainHero(tilesAndDirection.tiles))
// 		{
// 			return;
// 		}

		AnimationController.Instance.ShowTiles(tilesAndDirection.tiles, Tile.OverlayType.PossibleMovement, ReturnFromShowingTiles);
	}

	public void ReturnFromShowingTiles()
	{
		Tuple<int, int> dir = TileGrid.directions[(int)tilesAndDirection.direction];
		List<Tile> characterOrigin = TileGrid.Instance.FindCharacter(owningCharacter);
		Tile startTile = characterOrigin[0];
		List<Tile> trail = Trample.GetTramplePath(owningCharacter, startTile, dir, range);
		while (trail.Count > 2)
		{
			trail.RemoveAt(1);
		}
		TileGrid.Instance.RouteCharacterToTile(owningCharacter, trail, ReturnFromRoute, false, false);

	}

	public void ReturnFromRoute()
	{
		List<Tile> characterOrigin = TileGrid.Instance.FindCharacter(owningCharacter);
		Tile startTile = characterOrigin[0];
		Tuple<int, int> dir = TileGrid.directions[(int)tilesAndDirection.direction];
		int i = 1;
		for (; i <= range; ++i)
		{
			Tile newTile = TileGrid.Instance.GetTile(startTile.x + dir.Item1 * i, startTile.y + dir.Item2 * i);
			if (newTile == null)
			{
				break;
			}
			if (TileGrid.Instance.WhatTilesWouldCharacterTake(owningCharacter, newTile) == null)
			{
				break;
			}

		}
		--i;
		Tile newPlacementTile = TileGrid.Instance.GetTile(startTile.x + dir.Item1 * i, startTile.y + dir.Item2 * i);
		List<Tile> impactTiles = TileGrid.Instance.WhatTilesWouldCharacterTake(owningCharacter, newPlacementTile);
		hitCharacters = new List<Character>();
		foreach (Tile t in tilesAndDirection.tiles)
		{
			t.HideOverlay(Tile.OverlayType.PossibleMovement);

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
		TileGrid.Instance.MoveCharacterToTile(owningCharacter, newPlacementTile);
		owningCharacter.SetFacing(direction);
		hitCharacters = hitCharacters.Distinct().ToList();
		int damageDiceCount = 1;
		int damageDiceFace = 4;
		foreach (Character c in hitCharacters)
		{
			ActionController.AttackResults results = new ActionController.AttackResults();
			ActionController.Instance.DamageCharacter(c, owningCharacter, new ActionController.AttackProfile(damageDiceCount, damageDiceFace, 0), results);
			c.AddStatusEffect(typeof(Stun), null);
		}
		callback.Invoke();
	}
}
