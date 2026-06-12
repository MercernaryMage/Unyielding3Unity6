using System;
using System.Collections.Generic;
using UnityEngine;

public class Guillotine : Card
{
    Tuple<List<Tile>, Tile> route;
    List<Tile> targetTiles;

    public override void Execute()
    {
        Dictionary<Character, Tuple<List<Tile>, Tile>> routes = RouteToAllClosestCharacters(true);
        route = Util.FindSmallestRoute(routes, null);

        if (route == null)
        {
            Debug.Log("No possible route");
            Finish();
            return;
        }

		targetTiles = TemplateLibrary.GetAdjacentCharacterTarget(owningCharacter, null);
		if (targetTiles != null)
        {
            if (targetTiles == null)
            {
                Finish();
                return;
            }
            AnimationController.Instance.ShowTiles(targetTiles, Tile.OverlayType.PossibleAttck, ReturnFromShowingAttackTiles);
        }
        else
        {
            Util.ShortenPathToMaxRange(route, owningCharacter.characterDefinition.movement + 1);
            AnimationController.Instance.ShowTiles(route.Item1, Tile.OverlayType.PossibleMovement, ReturnFromShowingMovementTiles, ReturnFromRoute);
        }
    }

    void ReturnFromShowingMovementTiles()
    {
        TileGrid.Instance.RouteAICharacterToTile(owningCharacter, new List<Tile>(route.Item1), ReturnFromRoute);
    }

    void ReturnFromRoute()
    {
        foreach (Tile t in route.Item1)
        {
            t.HideOverlay(Tile.OverlayType.PossibleMovement);
        }
        Finish();
    }

    void ReturnFromShowingAttackTiles()
    {
        owningCharacter.SetFacing(TileGrid.Instance.GetFacingDirection(owningCharacter, targetTiles[0].character));
        ActionController.Instance.PlayAttackAnimation(owningCharacter, null, () =>
        {
            foreach (Tile t in targetTiles)
            {
                if (t.character != null && t.character.hero)
                {
                    ActionController.Instance.AttackCharacter(t.character, owningCharacter, new ActionController.AttackProfile(0, 0, 10));
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
        instructions.Add(new CardInstruction("Move to closest enemy"));
        instructions.Add(new CardInstruction("If already adjacent, attack for 10 damage"));
        DisplayGrid.Instance.Show();
        return instructions;
    }
}
