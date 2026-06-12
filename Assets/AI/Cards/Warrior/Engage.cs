using System;
using System.Collections.Generic;
using UnityEngine;

public class Engage : Card
{
    Tuple<List<Tile>, Tile> route;

    public override void Execute()
    {
        Dictionary<Character, Tuple<List<Tile>, Tile>> routes = RouteToAllClosestCharacters(true);
        route = Util.FindSmallestRoute(routes, null);

        if (route == null)
        {
            Finish();
            return;
        }

        Util.ShortenPathToMaxRange(route, owningCharacter.characterDefinition.movement + 1);
        AnimationController.Instance.ShowTiles(route.Item1, Tile.OverlayType.PossibleMovement, ReturnFromShowingTiles, ReturnFromRoute);
    }

    void ReturnFromShowingTiles()
    {
        TileGrid.Instance.RouteAICharacterToTile(owningCharacter, new List<Tile>(route.Item1), ReturnFromRoute);
    }

    void ReturnFromRoute()
    {
        foreach (Tile t in route.Item1)
        {
            t.HideOverlay(Tile.OverlayType.PossibleMovement);
        }

        foreach (Tile t in TileGrid.Instance.GetAllAdjacentTilesToCharacter(owningCharacter))
        {
            if (t.character != null && t.character.hero && !t.character.IsDowned())
            {
                t.character.AddStatusEffect(typeof(Jammed), null);
            }
        }

        owningCharacter.AddStatusEffect(typeof(Waiting), null);

        Finish();
    }

    public static List<CardInstruction> GetCardInstructions(CardScriptableObject scriptableObject)
    {
        DisplayGrid.Instance.Clear(11, 8);
        List<CardInstruction> instructions = new List<CardInstruction>();
        instructions.Add(new CardInstruction("Move toward closest enemy"));
        instructions.Add(new CardInstruction("Give adjacent enemies Jammed"));
        instructions.Add(new CardInstruction("Give self Waiting"));
        DisplayGrid.Instance.Show();
        return instructions;
    }
}
