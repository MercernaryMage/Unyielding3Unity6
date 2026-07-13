using System.Collections.Generic;
using UnityEngine;

public class PickTarget : Card
{
    Character target;
    List<Tile> movePath;
    List<Tile> litRouteTiles;

    public override void Execute()
    {
        bool enemyWithinThree = false;
        foreach (Character hero in GetLivingEnemies())
        {
            if (TileGrid.Instance.GetDistanceBetweenCharacters(hero, owningCharacter) <= 3)
            {
                enemyWithinThree = true;
                break;
            }
        }

        if (enemyWithinThree)
        {
            MoveAwayIfNeeded(PickLockOnTarget);
            return;
        }

        foreach (Character hero in GetLivingEnemies())
        {
            if (TileGrid.Instance.DoesCharacterHaveLOSToCharacter(owningCharacter, hero))
            {
                PickLockOnTarget();
                return;
            }
        }

        MoveToLineOfSightTile();
    }

    List<Character> GetLivingEnemies()
    {
        List<Character> enemies = new List<Character>();
        foreach (Character hero in BattleController.Instance.heroes)
        {
            if (!hero.alive || hero.GetComponent<Downed>() != null)
            {
                continue;
            }
            enemies.Add(hero);
        }
        return enemies;
    }

    //move to the closest reachable tile that has line of sight to an enemy,
    //skipping any tile with an enemy within 3 tiles of it
    void MoveToLineOfSightTile()
    {
        List<Character> enemies = GetLivingEnemies();

        int moveRange = owningCharacter.characterDefinition.movement;
        MovementController.PathfindingRules rules = new MovementController.PathfindingRules();
        rules.allowedToPathThroughAllies = true;

        List<List<Tile>> bestRoutes = new List<List<Tile>>();
        int bestLength = int.MaxValue;

        foreach (Tile tile in TileGrid.Instance.tiles)
        {
            if (tile.character != null)
            {
                continue;
            }
            if (!TileGrid.Instance.WouldCharacterFitAtTile(owningCharacter, tile))
            {
                continue;
            }

            bool enemyTooClose = false;
            bool hasLOS = false;
            foreach (Character enemy in enemies)
            {
                foreach (Tile enemyTile in TileGrid.Instance.FindCharacter(enemy))
                {
                    if (TileGrid.Distance(tile, enemyTile) <= 3)
                    {
                        enemyTooClose = true;
                        break;
                    }
                    if (!hasLOS && TileGrid.Instance.DoesTileHaveLOSToTile(tile, enemyTile))
                    {
                        hasLOS = true;
                    }
                }
                if (enemyTooClose)
                {
                    break;
                }
            }
            if (enemyTooClose || !hasLOS)
            {
                continue;
            }

            List<Tile> route = MovementController.Instance.FindRoute(owningCharacter, tile, 0, rules);
            if (route == null || route.Count > moveRange + 1)
            {
                continue;
            }

            if (route.Count < bestLength)
            {
                bestLength = route.Count;
                bestRoutes.Clear();
                bestRoutes.Add(route);
            }
            else if (route.Count == bestLength)
            {
                bestRoutes.Add(route);
            }
        }

        if (bestRoutes.Count == 0)
        {
            PickLockOnTarget();
            return;
        }

        movePath = bestRoutes[Random.Range(0, bestRoutes.Count)];
        if (movePath.Count <= 1)
        {
            PickLockOnTarget();
            return;
        }

        litRouteTiles = Util.ExpandPathTiles(movePath, owningCharacter);
        AnimationController.Instance.ShowTiles(litRouteTiles, Tile.OverlayType.PossibleMovement, Route, HideRouteAndPickTarget);
    }

    void Route()
    {
        TileGrid.Instance.RouteAICharacterToTile(owningCharacter, new List<Tile>(movePath), HideRouteAndPickTarget);
    }

    void HideRouteAndPickTarget()
    {
        foreach (Tile t in litRouteTiles)
        {
            t.HideOverlay(Tile.OverlayType.PossibleMovement);
        }
        PickLockOnTarget();
    }

    void PickLockOnTarget()
    {
        List<Character> candidates = new List<Character>();
        foreach (Character hero in GetLivingEnemies())
        {
            if (TileGrid.Instance.DoesCharacterHaveLOSToCharacter(owningCharacter, hero))
            {
                candidates.Add(hero);
            }
        }

        if (candidates.Count == 0)
        {
            Finish();
            return;
        }

        target = candidates[Random.Range(0, candidates.Count)];
        AnimationController.Instance.ScrollToCharacter(target, Delay, .5f);
    }

    void Delay()
    {
        LockedOn lockedOn = (LockedOn)target.AddStatusEffect(typeof(LockedOn), null);
        lockedOn.causingCharacter = owningCharacter;
        Finish();
    }

    public static List<CardInstruction> GetCardInstructions(CardScriptableObject scriptableObject)
    {
        DisplayGrid.Instance.Clear(11, 8);
        List<CardInstruction> instructions = new List<CardInstruction>();
        instructions.Add(new CardInstruction("If an enemy is within 3 tiles, move to the tile farthest from all enemies"));
        instructions.Add(new CardInstruction("Otherwise, if no enemy is in line of sight, move to the closest tile with line of sight that has no enemy within 3 tiles"));
        instructions.Add(new CardInstruction("Apply Locked On to a random enemy in line of sight"));
        return instructions;
    }
}
