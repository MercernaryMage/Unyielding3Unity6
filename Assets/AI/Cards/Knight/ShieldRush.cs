using System;
using System.Collections.Generic;
using UnityEngine;

public class ShieldRush : Card
{
    Tuple<List<Tile>, Tile> route;
    TemplateLibrary.TilesAndDirection attackTiles;

    public override void Execute()
    {
        route = FindRouteAdjacentToBoth();

        if (route == null)
        {
            Dictionary<Character, Tuple<List<Tile>, Tile>> allyRoutes = RouteToAllClosestCharacters(false);
            route = Util.FindSmallestRoute(allyRoutes, null);
        }

        if (route == null)
        {
            Dictionary<Character, Tuple<List<Tile>, Tile>> enemyRoutes = RouteToAllClosestCharacters(true);
            route = Util.FindSmallestRoute(enemyRoutes, null);
        }

        if (route == null)
        {
            Finish();
            return;
        }

        Util.ShortenPathToMaxRange(route, owningCharacter.characterDefinition.movement + 1);

        AnimationController.Instance.ShowTiles(route.Item1, Tile.OverlayType.PossibleMovement, ReturnFromShowingTiles, ReturnFromRoute);
    }

    public void ReturnFromShowingTiles()
    {
        TileGrid.Instance.RouteAICharacterToTile(owningCharacter, new List<Tile>(route.Item1), ReturnFromRoute);
    }

    public void ReturnFromRoute()
    {
        foreach (Tile t in route.Item1)
        {
            t.HideOverlay(Tile.OverlayType.PossibleMovement);
        }

        List<Tile> tiles = TemplateLibrary.GetAdjacentCharacterTarget(owningCharacter, null);
        if (tiles == null)
        {
            Finish();
            return;
        }

        attackTiles = new TemplateLibrary.TilesAndDirection(tiles, Direction.East);
        AnimationController.Instance.ShowTiles(attackTiles.tiles, Tile.OverlayType.PossibleAttck, ReturnFromShowingAttackTiles);
    }

    public void ReturnFromShowingAttackTiles()
    {
        owningCharacter.SetFacing(TileGrid.Instance.GetFacingDirection(owningCharacter, attackTiles.tiles[0].character));
        ActionController.Instance.PlayAttackAnimation(owningCharacter, null, () =>
        {
            foreach (Tile t in attackTiles.tiles)
            {
                if (t.character != null && t.character.hero)
                {
                    ActionController.Instance.AttackCharacter(t.character, owningCharacter, new ActionController.AttackProfile(1, 6, 0));
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
        instructions.Add(new CardInstruction("Move adjacent to both an ally and an enemy"));
        instructions.Add(new CardInstruction("or adjacent to closest ally if not possible"));
		instructions.Add(new CardInstruction("or adjacent to closest enemy if not possible"));
		instructions.Add(new CardInstruction("If adjacent to enemy after moving,"));
        instructions.Add(new CardInstruction("attack for 1d6 damage"));
        DisplayGrid.Instance.Show();

        return instructions;
    }
}
