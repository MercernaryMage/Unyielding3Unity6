using System.Collections.Generic;
using UnityEngine;

public class Freightened : StatusEffect
{
	public override void CharacterStartTurn(CharacterStartTurnMessage message)
	{
		if (message.character != character)
		{
			return;
		}

		List<Character> threats = BattleController.Instance.GetEnemies(character);

		Tile bestTile = null;
		List<Tile> bestRoute = null;
		int bestMinDist = -1;

		MovementController.PathfindingRules rules = new MovementController.PathfindingRules();
		rules.allowedToPathThroughAllies = true;

		foreach (Tile tile in TileGrid.Instance.tiles)
		{
			if (tile.character != null)
			{
				continue;
			}


			if (TileGrid.Instance.GetDistanceBetweenCharacterAndTile(character, tile) > character.currentMovement)
			{
				continue; 
			}
			List<Tile> route = MovementController.Instance.FindRoute(character, tile, 0, rules);
			if (route == null || route.Count > character.currentMovement + 1)
			{
				continue;
			}

			int minDist = int.MaxValue;
			foreach (Character threat in threats)
			{
				if (!threat.alive)
				{
					continue;
				}
				List<Tile> threatTiles = TileGrid.Instance.FindCharacter(threat);
				foreach (Tile threatTile in threatTiles)
				{
					int dist = TileGrid.Distance(tile, threatTile);
					if (dist < minDist)
					{
						minDist = dist;
					}
				}
			}

			if (minDist > bestMinDist)
			{
				bestMinDist = minDist;
				bestTile = tile;
				bestRoute = route;
			}
		}

		if (bestRoute == null || bestRoute.Count <= 1)
		{
			Destroy(this);
			return;
		}

		message.turnStartLocks.Add(this);
		BattleController.playerHasControl = false;

		TileGrid.Instance.RouteCharacterToTile(character, bestRoute, () =>
		{
			BattleController.playerHasControl = true;
			TurnControl.Instance.RemoveLock(this);
			Destroy(this);
		});
	}

	public override string GetDisplayName()
	{
		return "Freightened";
	}

	public override string GetEffectText()
	{
		return "At the start of your turn, move to the farthest reachable tile from all enemies, then remove this effect.";
	}
}
