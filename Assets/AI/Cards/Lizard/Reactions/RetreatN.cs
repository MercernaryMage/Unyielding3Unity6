using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RetreatN : ReactionBase
{
	public override void Execute()
	{
		AnimationController.Instance.ShowTiles(null, Tile.OverlayType.PossibleMovement, Delay);
	}

	public void Delay()
	{
		Direction newFacing = TileGrid.GetOppositeFacing(owningCharacter.facing);
		Tuple<int, int> direction = TileGrid.directions[(int)newFacing];

		List<Tile> tiles = TileGrid.Instance.FindCharacter(owningCharacter);
		for (int i = 4; i > 0; --i)
		{
			Tile newTile = TileGrid.Instance.GetTile(tiles[0].x + direction.Item1 * i, tiles[0].y + direction.Item2 * i);
			if (newTile == null)
			{
				continue;
			}
			if (TileGrid.Instance.WouldCharacterFitAtTile(owningCharacter, newTile))
			{
				//newTile
				List<Tile> route = new List<Tile>()
				{
					tiles[0],
					newTile
				};
				TileGrid.Instance.RouteCharacterToTile(owningCharacter, new List<Tile>(route), BattleController.ReturnControlToPlayer, true, false);
				
				return;
			}
		}
		BattleController.ReturnControlToPlayer();
	}

	public static List<CardInstruction> GetCardInstructions(CardScriptableObject scriptableObject)
	{
		DisplayGrid.Instance.Clear(12, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction("Move backwards up to 4 spaces"));
		return instructions;
	}
}
