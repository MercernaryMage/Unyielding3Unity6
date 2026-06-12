using System;
using System.Collections.Generic;
using UnityEngine;

public class Swoop : Card
{
	TemplateLibrary.TilesAndDirection tilesAndDirection;

	public override void Execute()
	{
		int size = owningCharacter.characterDefinition.size;
		tilesAndDirection = TemplateLibrary.Instance.GetMostCharactersInCardinalDirections(owningCharacter, size);

		if (!TileGrid.TilesContainHero(tilesAndDirection.tiles))
		{
			List<Tile> facingTiles = TemplateLibrary.Instance.GetTilesInMatchedSizeCardinalDirection(owningCharacter, size, (int)owningCharacter.facing);
			tilesAndDirection = new TemplateLibrary.TilesAndDirection(facingTiles, owningCharacter.facing);
		}

		AnimationController.Instance.ShowTiles(tilesAndDirection.tiles, Tile.OverlayType.PossibleMovement, ReturnFromShowingTiles);
	}

	void ReturnFromShowingTiles()
	{
		int size = owningCharacter.characterDefinition.size;
		Tuple<int, int> dir = TileGrid.directions[(int)tilesAndDirection.direction];
		Tile startTile = TileGrid.Instance.FindCharacter(owningCharacter)[0];
		List<Tile> trail = Trample.GetTramplePath(owningCharacter, startTile, dir, size);
		TileGrid.Instance.RouteAICharacterToTile(owningCharacter, trail, ReturnFromRoute, false, false);
	}

	void ReturnFromRoute()
	{
		int size = owningCharacter.characterDefinition.size;
		Trample.DoTrample(owningCharacter, tilesAndDirection, size, Finish);
	}

	public static List<CardInstruction> GetCardInstructions(CardScriptableObject scriptableObject)
	{
		DisplayGrid.Instance.Clear(11, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction("Lunge one body-length in the direction"));
		instructions.Add(new CardInstruction("that hits the most enemies"));
		instructions.Add(new CardInstruction());
		instructions.Add(new CardInstruction("Deal 1d6 damage to each target"));
		instructions.Add(new CardInstruction("Stun on hit"));
		DisplayGrid.Instance.Show();
		return instructions;
	}
}
