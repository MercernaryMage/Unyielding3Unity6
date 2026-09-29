using System;
using System.Collections.Generic;
using UnityEngine;

public class ShieldRush : Card
{
    Tuple<List<Tile>, Tile> route;
    List<Tile> litRouteTiles;
    TemplateLibrary.TilesAndDirection attackTiles;

    public override void Execute()
    {
        int maxRange = owningCharacter.characterDefinition.movement + 1;

        Tuple<List<Tile>, Tile> bothRoute = FindRouteAdjacentToBoth();
        Dictionary<Character, Tuple<List<Tile>, Tile>> allyRoutes = null;
        Dictionary<Character, Tuple<List<Tile>, Tile>> enemyRoutes = null;

        route = null;
        if (bothRoute != null && bothRoute.Item1.Count <= maxRange)
        {
            route = bothRoute;
        }

        if (route == null)
        {
            allyRoutes = RouteToAllClosestCharacters(false);
            Dictionary<Character, Tuple<List<Tile>, Tile>> reachableAllyRoutes = new Dictionary<Character, Tuple<List<Tile>, Tile>>(allyRoutes);
            Util.RemoveOutOfRangeRoutes(reachableAllyRoutes, maxRange);
            route = Util.FindSmallestRoute(reachableAllyRoutes, null);
        }

        if (route == null)
        {
            enemyRoutes = RouteToAllClosestCharacters(true);
            Dictionary<Character, Tuple<List<Tile>, Tile>> reachableEnemyRoutes = new Dictionary<Character, Tuple<List<Tile>, Tile>>(enemyRoutes);
            Util.RemoveOutOfRangeRoutes(reachableEnemyRoutes, maxRange);
            route = Util.FindSmallestRoute(reachableEnemyRoutes, null);
        }

        if (route == null)
        {
            route = bothRoute;
        }

        if (route == null)
        {
            route = Util.FindSmallestRoute(allyRoutes, null);
        }

        if (route == null)
        {
            route = Util.FindSmallestRoute(enemyRoutes, null);
        }

        if (route == null)
        {
            Finish();
            return;
        }

        Util.ShortenPathToMaxRange(route, maxRange);

        litRouteTiles = Util.ExpandPathTiles(route.Item1, owningCharacter);
        AnimationController.Instance.ShowTiles(litRouteTiles, Tile.OverlayType.PossibleMovement, ReturnFromShowingTiles, ReturnFromRoute);
    }

    public void ReturnFromShowingTiles()
    {
        TileGrid.Instance.RouteAICharacterToTile(owningCharacter, new List<Tile>(route.Item1), ReturnFromRoute);
    }

    public void ReturnFromRoute()
    {
        foreach (Tile t in litRouteTiles)
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
