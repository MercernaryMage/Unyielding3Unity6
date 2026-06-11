using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HowlAndGrowl : Card
{
	TemplateLibrary.TilesAndDirection tilesAndDirection;
	List<Character> hitCharacters;
	Tuple<List<Tile>, Tile> route;
	Tile startTile;

	//Find closest enemy

	public override void Execute()
	{
		AnimationController.Instance.ShowTiles(null, Tile.OverlayType.PossibleMovement, Delay);
	}

	public void Delay()
	{
		Howl(owningCharacter, 1);
		Growl(owningCharacter, 2);
		Finish();
	}

	public static List<CardInstruction> GetCardInstructions(CardScriptableObject scriptableObject)
	{
		DisplayGrid.Instance.Clear(11, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction("Howl"));
		instructions.Add(new CardInstruction("Growl"));
		DisplayGrid.Instance.Show();




		return instructions;
	}

	public static void Howl(Character howlingCharacter, int value)
	{
		List<Tuple<Tile,Direction>> edgeTiles = new List<Tuple<Tile, Direction>>();
		for (int i = 0; i < TileGrid.Instance.width; ++i)
		{
			Tile t = TileGrid.Instance.GetTile(i, 0);
			if (t.character == null && t.tileScriptableObject.enterable)
			{
				edgeTiles.Add(new Tuple<Tile, Direction>(t, Direction.North));
			}
			Tile t2 = TileGrid.Instance.GetTile(i, TileGrid.Instance.height - 1);
			if (t2.character == null && t2.tileScriptableObject.enterable)
			{
				edgeTiles.Add(new Tuple<Tile, Direction>(t2, Direction.South));
			}
		}
		for (int i = 0; i < TileGrid.Instance.height; ++i)
		{
			Tile t = TileGrid.Instance.GetTile(0, i);
			if (t.character == null && t.tileScriptableObject.enterable)
			{
				edgeTiles.Add(new Tuple<Tile, Direction>(t, Direction.East));
			}
			Tile t2 = TileGrid.Instance.GetTile(TileGrid.Instance.width - 1, i);
			if (t2.character == null && t2.tileScriptableObject.enterable)
			{
				edgeTiles.Add(new Tuple<Tile, Direction>(t2, Direction.West));
			}
		}
		value = Mathf.Min(value, edgeTiles.Count);
		for (int i = 0; i < value; ++i)
		{
			int index = UnityEngine.Random.Range(0, edgeTiles.Count);
			Tuple<Tile, Direction> t = edgeTiles[index];
			edgeTiles.RemoveAt(index);
			Character newCharacter = MapBuilder.PlaceEnemy(t.Item1.x, t.Item1.y, CharacterRepository.Instance.GetCharacter("Weak Bandit"), t.Item2);
			newCharacter.AddStatusEffect(typeof(Stun), null);
			TurnControl.Instance.AddCharacter(newCharacter);
		}
	}

	public static void Growl(Character growlingCharacer, int value)
	{
		foreach (Reaction reaction in growlingCharacer.reactions)
		{
			if (!reaction.isAggravatged)
			{
				reaction.isAggravatged = true;
				--value;
				if (value == 0)
				{
					return;
				}
			}
		}
	}
}
