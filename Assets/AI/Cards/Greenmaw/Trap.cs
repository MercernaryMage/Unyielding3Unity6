using System;
using System.Collections.Generic;
using UnityEngine;

public class Trap : Card
{
	Tuple<List<Tile>, Tile> route;
	List<Tile> litRouteTiles;
	TemplateLibrary.TilesAndDirection tilesAndDirection;

	bool applyStatus;
	int attackDice;

	bool IsNotStaked(Character target)
	{
		if (target.GetComponent<Downed>())
		{
			return false;
		}
		return target.GetComponent<Staked>() == null;
	}

	public override void Execute()
	{
		Dictionary<Character, Tuple<List<Tile>, Tile>> routes = RouteToAllClosestCharacters(true);
		int range = owningCharacter.characterDefinition.movement + 1;

		//Primary: hunt the closest non-staked enemy we can actually reach this turn.
		Dictionary<Character, Tuple<List<Tile>, Tile>> inRange = new Dictionary<Character, Tuple<List<Tile>, Tile>>(routes);
		Util.RemoveOutOfRangeRoutes(inRange, range);
		route = Util.FindSmallestRoute(inRange, IsNotStaked);

		if (route != null)
		{
			applyStatus = true;
			attackDice = 1;
		}
		else
		{
			//Fallback: no valid target in range, so close on the nearest enemy and hit harder.
			route = Util.FindSmallestRoute(routes, null);
			applyStatus = false;
			attackDice = 2;
		}

		if (route == null)
		{
			Debug.Log("No possible route");
			Finish();
			return;
		}

		Util.ShortenPathToMaxRange(route, range);

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

		List<Tile> tiles = TemplateLibrary.GetAdjacentCharacterTarget(owningCharacter, null);
		if (tiles == null)
		{
			Debug.Log("No targets");
			Finish();
			return;
		}
		tilesAndDirection = new TemplateLibrary.TilesAndDirection(tiles, Direction.East);

		AnimationController.Instance.ShowTiles(tilesAndDirection.tiles, Tile.OverlayType.PossibleAttck, ReturnFromShowingAttackTiles);
	}

	public void ReturnFromShowingAttackTiles()
	{
		owningCharacter.SetFacing(TileGrid.Instance.GetFacingDirection(owningCharacter, tilesAndDirection.tiles[0].character));
		ActionController.Instance.PlayAttackAnimation(owningCharacter, null, () =>
		{
			foreach (Tile t in tilesAndDirection.tiles)
			{
				if (t.character != null && t.character.hero)
				{
					ActionController.AttackResults results = ActionController.Instance.AttackCharacter(
						t.character, owningCharacter, new ActionController.AttackProfile(attackDice, 6, 0));
					if (applyStatus && results.hit)
					{
						t.character.AddStatusEffect(typeof(Staked), null);
						t.character.AddStatusEffect(typeof(Immobilize), null);
					}
					t.HideOverlay(Tile.OverlayType.PossibleAttck);
				}
			}

			Finish();
		});
	}

	public static List<CardInstruction> GetCardInstructions(CardScriptableObject scriptableObject)
	{
		DisplayGrid.Instance.Clear(11, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction("Move to the closest enemy in range that is not <u>Staked</u>"));
		instructions.Add(new CardInstruction("Attack for 1d6 damage"));
		instructions.Add(new CardInstruction("On hit, apply <u>Staked</u> and <u>Immobilized</u>"));
		instructions.Add(new CardInstruction("If there is no target, move to the closest enemy and attack for 2d6 damage"));
		DisplayGrid.Instance.Show();

		return instructions;
	}
}
