using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RetreatA : ReactionBase
{
	TemplateLibrary.TilesAndDirection tilesAndDirection;

	public override void Execute()
	{
		AnimationController.Instance.ShowTiles(null, Tile.OverlayType.PossibleMovement, Delay);
	}

	public void Delay()
	{
		owningCharacter.SetFacing(TileGrid.Instance.GetFacingDirection(owningCharacter, attackingCharacter));
		int oppositeDirection = (int)TileGrid.Instance.GetFacingDirection(attackingCharacter, owningCharacter);
		List<Tile> tiles = TemplateLibrary.Instance.GetTilesInMatchedSizeCardinalDirection(owningCharacter, 4, oppositeDirection);
		tilesAndDirection = new TemplateLibrary.TilesAndDirection(tiles, (Direction)oppositeDirection);
		AnimationController.Instance.ShowTiles(tilesAndDirection.tiles, Tile.OverlayType.PossibleMovement, ReturnFromShowingTiles);
	}

	public void ReturnFromShowingTiles()
	{
		Trample.DoTrample(owningCharacter, tilesAndDirection, 4, ReturnFromTrample, 6);
	}

	public void ReturnFromTrample()
	{
		BattleController.ReturnControlToPlayer();
	}

	public static List<CardInstruction> GetCardInstructions(CardScriptableObject scriptableObject, Character character )
	{
		DisplayGrid.Instance.Clear(12, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction("Move 4 spaces directly away from attacker."));
		instructions.Add(new CardInstruction("Deal 1d6 damage and apply <u>Stunned</u> to any character in the way."));
		return instructions;
	}
}
