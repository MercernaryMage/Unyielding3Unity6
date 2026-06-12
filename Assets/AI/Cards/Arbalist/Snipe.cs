using System;
using System.Collections.Generic;
using UnityEngine;

public class Snipe : Card
{
    List<Tile> movePath;
    List<Tile> attackLine;
    Character targetHero;

    public override void Execute()
    {
        List<Tile> myTiles = TileGrid.Instance.FindCharacter(owningCharacter);
        Tile origin = myTiles[0];
        int moveRange = owningCharacter.characterDefinition.movement;

        MovementController.PathfindingRules rules = new MovementController.PathfindingRules();
        rules.allowedToPathThroughAllies = true;

        Tile bestTile = null;
        Character bestHero = null;
        List<Tile> bestPath = null;
        int bestPathLength = int.MaxValue;

        List<Tile> candidates = TileGrid.Instance.GetAllTilesInRange(origin, moveRange);
        if (!candidates.Contains(origin))
        {
            candidates.Add(origin);
        }

        foreach (Tile candidate in candidates)
        {
            List<Tile> path;
            if (candidate == origin)
            {
                path = new List<Tile>();
            }
            else
            {
                if (!TileGrid.Instance.WouldCharacterFitAtTile(owningCharacter, candidate))
                {
                    continue;
                }

                path = MovementController.Instance.FindRoute(owningCharacter, candidate, 0, rules);
                if (path == null || path.Count > moveRange + 1)
                {
                    continue;
                }
            }

            foreach (Character hero in BattleController.Instance.heroes)
            {
                if (!hero.alive || hero.GetComponent<Downed>() != null)
                {
                    continue;
                }

                foreach (Tile heroTile in TileGrid.Instance.FindCharacter(hero))
                {
                    if (TileGrid.Distance(candidate, heroTile) <= 1)
                    {
                        continue;
                    }

                    List<Tile> collisions = TileGrid.Instance.GetLineCollisions(candidate, heroTile);
                    bool blocked = false;
                    foreach (Tile collision in collisions)
                    {
                        if (collision.tileScriptableObject.blocksLOS)
                        {
                            blocked = true;
                            break;
                        }
                    }

                    if (!blocked && path.Count < bestPathLength)
                    {
                        bestPathLength = path.Count;
                        bestTile = candidate;
                        bestHero = hero;
                        bestPath = path;
                    }
                }
            }
        }

        if (bestTile == null)
        {
            Finish();
            return;
        }

        targetHero = bestHero;
        movePath = bestPath;

        if (movePath.Count == 0)
        {
            ShowAttack();
        }
        else
        {
            AnimationController.Instance.ShowTiles(movePath, Tile.OverlayType.PossibleMovement, Route, ShowAttack);
        }
    }

    void Route()
    {
        TileGrid.Instance.RouteAICharacterToTile(owningCharacter, new List<Tile>(movePath), ShowAttack);
    }

    void ShowAttack()
    {
        foreach (Tile t in movePath)
        {
            t.HideOverlay(Tile.OverlayType.PossibleMovement);
        }

        foreach (Character hero in BattleController.Instance.heroes)
        {
            if (!hero.alive || hero.GetComponent<Downed>() != null)
            {
                continue;
            }

            foreach (Tile heroTile in TileGrid.Instance.FindCharacter(hero))
            {
                if (TileGrid.Instance.GetDistanceBetweenCharacters(hero, owningCharacter) <= 1)
                {
                    FloatingCombatNumberController.Instance.QueueFloatingCombatNumber(owningCharacter, "Interrupted");
                    Finish();
                    return;
                }
            }
        }

        Tile shootOrigin = TileGrid.Instance.FindCharacter(owningCharacter)[0];
        Tile targetHeroTile = TileGrid.Instance.FindCharacter(targetHero)[0];

        attackLine = TileGrid.Instance.GetLineCollisions(shootOrigin, targetHeroTile);
        attackLine.Sort((a, b) =>
            Vector3.Distance(shootOrigin.transform.position, a.transform.position)
            .CompareTo(Vector3.Distance(shootOrigin.transform.position, b.transform.position)));

        owningCharacter.SetFacing(TileGrid.Instance.GetFacingDirection(owningCharacter, targetHero));

        AnimationController.Instance.ShowTiles(attackLine, Tile.OverlayType.PossibleAttck, FireShot);
    }

    void FireShot()
    {
        Character firstTarget = null;
        foreach (Tile t in attackLine)
        {
            t.HideOverlay(Tile.OverlayType.PossibleAttck);
            if (firstTarget == null && t.character != null && t.character.hero)
            {
                firstTarget = t.character;
            }
        }

        if (firstTarget == null)
        {
            firstTarget = targetHero;
        }

		Targeted targeted = (Targeted)firstTarget.AddStatusEffect(Type.GetType("Targeted"), null);
        targeted.causingCharacter = owningCharacter;

		Finish();        
    }

    public static List<CardInstruction> GetCardInstructions(CardScriptableObject scriptableObject)
    {
        DisplayGrid.Instance.Clear(11, 8);
        List<CardInstruction> instructions = new List<CardInstruction>();
        instructions.Add(new CardInstruction("Move to the closest tile with line of sight to a hero"));
        instructions.Add(new CardInstruction("Deal 10 damage to the first hero in the line"));
        DisplayGrid.Instance.Show();
        return instructions;
    }
}
