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
        Dictionary<Character, Tuple<List<Tile>, Tile>> enemyRoutes = null;

        route = null;
        if (bothRoute != null && bothRoute.Item1.Count <= maxRange)
        {
            route = bothRoute;
        }

        if (route == null)
        {
            route = FindAllyAdjacentRouteClosestToEnemy(maxRange);
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
            route = FindAllyAdjacentRouteClosestToEnemy(int.MaxValue);
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

    Tuple<List<Tile>, Tile> FindAllyAdjacentRouteClosestToEnemy(int maxRange)
    {
        Tuple<List<Tile>, Tile> bestRoute = null;
        int bestEnemyDistance = int.MaxValue;
        int tileSize = owningCharacter.characterDefinition.size * owningCharacter.characterDefinition.size;

        foreach (Character ally in BattleController.Instance.enemies)
        {
            if (ally == owningCharacter)
            {
                continue;
            }
            if (!ally.alive)
            {
                continue;
            }
            if (ally.gameObject.GetComponent<Downed>() != null)
            {
                continue;
            }

            List<Tile> adjacentToAlly = TileGrid.Instance.GetAllAdjacentTilesToCharacter(ally);

            for (int i = 0; i < tileSize; ++i)
            {
                foreach (Tile t in adjacentToAlly)
                {
                    Tile trueTile = TileGrid.Instance.GetTrueTileFromOffset(t, i, owningCharacter.characterDefinition.size);
                    if (trueTile == null)
                    {
                        continue;
                    }
                    if (!TileGrid.Instance.WouldCharacterFitAtTile(owningCharacter, trueTile))
                    {
                        continue;
                    }

                    int enemyDistance = GetClosestEnemyDistanceToTile(trueTile);
                    if (bestRoute != null && enemyDistance > bestEnemyDistance)
                    {
                        continue;
                    }

                    MovementController.PathfindingRules rules = new MovementController.PathfindingRules();
                    rules.allowedToPathThroughAllies = true;

                    List<Tile> path = MovementController.Instance.FindRoute(owningCharacter, trueTile, 0, rules);
                    if (path == null)
                    {
                        continue;
                    }
                    if (path.Count > maxRange)
                    {
                        continue;
                    }

                    if (bestRoute == null || enemyDistance < bestEnemyDistance || path.Count < bestRoute.Item1.Count)
                    {
                        bestRoute = new Tuple<List<Tile>, Tile>(path, trueTile);
                        bestEnemyDistance = enemyDistance;
                    }
                }
            }
        }

        return bestRoute;
    }

    int GetClosestEnemyDistanceToTile(Tile tile)
    {
        int closestDistance = int.MaxValue;

        foreach (Character hero in BattleController.Instance.heroes)
        {
            if (!hero.alive)
            {
                continue;
            }
            if (hero.gameObject.GetComponent<Downed>() != null)
            {
                continue;
            }

            int distance = TileGrid.Instance.GetDistanceBetweenCharacterAndTile(hero, tile);
            if (distance < closestDistance)
            {
                closestDistance = distance;
            }
        }

        return closestDistance;
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
        instructions.Add(new CardInstruction("or adjacent to an ally, nearest an enemy"));
		instructions.Add(new CardInstruction("or adjacent to closest enemy if not possible"));
		instructions.Add(new CardInstruction("If adjacent to enemy after moving,"));
        instructions.Add(new CardInstruction("attack for 1d6 damage"));
        DisplayGrid.Instance.Show();

        return instructions;
    }
}
