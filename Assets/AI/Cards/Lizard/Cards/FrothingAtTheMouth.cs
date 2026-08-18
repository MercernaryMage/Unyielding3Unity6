using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FrothingAtTheMouth : Card
{
	TemplateLibrary.TilesAndDirection tilesAndDirection;
	Tuple<List<Tile>, Tile> route;
	List<Tile> litRouteTiles;
	Character target;

	public override void Execute()
	{
		owningCharacter.AddStatusEffect(typeof(Frothing), null);

		Dictionary<Character, Tuple<List<Tile>, Tile>> routes = RouteToAllClosestCharacters(true);
		int maxRange = owningCharacter.characterDefinition.movement + 1;

		List<Character> reachable = new List<Character>();
		foreach (KeyValuePair<Character, Tuple<List<Tile>, Tile>> pair in routes)
		{
			if (!pair.Key.alive || pair.Value == null || pair.Value.Item1 == null)
			{
				continue;
			}
			if (pair.Value.Item1.Count > maxRange)
			{
				continue;
			}
			reachable.Add(pair.Key);
		}

		target = null;
		foreach (Character c in reachable)
		{
			if (c.GetComponent<Slobbered>() != null)
			{
				continue;
			}
			if (target == null || routes[c].Item1.Count < routes[target].Item1.Count)
			{
				target = c;
			}
		}

		if (target == null && reachable.Count > 0)
		{
			target = reachable[UnityEngine.Random.Range(0, reachable.Count)];
		}

		if (target == null)
		{
			Debug.Log("No possible route");
			Finish();
			return;
		}

		route = routes[target];
		Util.ShortenPathToMaxRange(route, maxRange);

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

		if (!TileGrid.AreCharactersAdjacent(target, owningCharacter))
		{
			Debug.Log("Not in range");
			Finish();
			return;
		}

		List<Tile> tiles = TileGrid.Instance.FindCharacter(target);
		if (tiles == null)
		{
			Debug.Log("No targets");
			Finish();
			return;
		}

		tilesAndDirection = new TemplateLibrary.TilesAndDirection(tiles, Direction.East);
		AnimationController.Instance.ShowTiles(tiles, Tile.OverlayType.PossibleAttck, ReturnFromShowingAttackTiles);
	}

	public void ReturnFromShowingAttackTiles()
	{
		owningCharacter.SetFacing(TileGrid.Instance.GetFacingDirection(owningCharacter, target));
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
		instructions.Add(new CardInstruction("Until a hero is <u>Downed</u>, all damage applies <u>Slobbered</u>"));
		instructions.Add(new CardInstruction("Move to closest enemy without <u>Slobbered</u> or a random reachable enemy if all have it"));
		instructions.Add(new CardInstruction("Attack the enemy for 1d6 damage"));

		DisplayGrid.Instance.Show();

		return instructions;
	}
}
