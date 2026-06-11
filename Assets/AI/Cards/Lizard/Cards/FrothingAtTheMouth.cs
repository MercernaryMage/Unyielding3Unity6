using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FrothingAtTheMouth : Card
{
	TemplateLibrary.TilesAndDirection tilesAndDirection;
	List<Character> hitCharacters;
	Tuple<List<Tile>, Tile> route;
	Tile startTile;

	public override void Execute()
	{
		AnimationController.Instance.ShowTiles(null, Tile.OverlayType.PossibleMovement, Delay, null, 3);
	}

	void Delay() 
	{
		owningCharacter.AddStatusEffect(typeof(Frothing), null);
		AIController.Instance.Reshuffle(owningCharacter);
		AIController.Instance.TakeTurn(owningCharacter);
	}

	public static List<CardInstruction> GetCardInstructions(CardScriptableObject scriptableObject)
	{
		DisplayGrid.Instance.Clear(11, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction("Until a hero is downed, all damage causes slobber"));
		instructions.Add(new CardInstruction("Do next action"));

		return instructions;
	}
}
