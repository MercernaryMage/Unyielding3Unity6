using System.Collections.Generic;
using UnityEngine;

public class DebugRun : Card
{
	List<Tile> route;
	List<Tile> litRouteTiles;

	public override void Execute()
	{
		int moveRange = owningCharacter.characterDefinition.movement;
		MovementController.PathfindingRules rules = new MovementController.PathfindingRules();
		rules.allowedToPathThroughAllies = true;

		route = null;

		foreach (Tile tile in TileGrid.Instance.tiles)
		{
			if (tile.character != null)
			{
				continue;
			}
			if (!TileGrid.Instance.WouldCharacterFitAtTile(owningCharacter, tile))
			{
				continue;
			}

			List<Tile> candidate = MovementController.Instance.FindRoute(owningCharacter, tile, 0, rules, moveRange);
			if (candidate == null || candidate.Count > moveRange + 1)
			{
				continue;
			}

			if (route == null || candidate.Count > route.Count)
			{
				route = candidate;
			}
		}

		if (route == null || route.Count <= 1)
		{
			ShowNoTarget(owningCharacter.transform.position);
			Finish();
			return;
		}

		litRouteTiles = Util.ExpandPathTiles(route, owningCharacter);
		AnimationController.Instance.ShowTiles(litRouteTiles, Tile.OverlayType.PossibleMovement, ReturnFromShowingTiles, ReturnFromRoute);
	}

	void ReturnFromShowingTiles()
	{
		TileGrid.Instance.RouteAICharacterToTile(owningCharacter, new List<Tile>(route), ReturnFromRoute);
	}

	void ReturnFromRoute()
	{
		foreach (Tile t in litRouteTiles)
		{
			t.HideOverlay(Tile.OverlayType.PossibleMovement);
		}

		Finish();
	}

	public static List<CardInstruction> GetCardInstructions(CardScriptableObject scriptableObject)
	{
		DisplayGrid.Instance.Clear(11, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction("Move to the farthest reachable tile"));
		DisplayGrid.Instance.Show();
		return instructions;
	}
}
