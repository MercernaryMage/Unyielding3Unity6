using System;
using System.Collections.Generic;
using UnityEngine;

public class Snipe : Card
{
    List<Tile> movePath;
    List<Tile> litRouteTiles;
    List<Tile> attackLine;
    Character targetHero;

	public override void Execute()
	{
		MoveAwayIfNeeded(FinishedRunning);
	}

	public void FinishedRunning(bool moved)
    {
		List<Tile> myTiles = TileGrid.Instance.FindCharacter(owningCharacter);
        Tile origin = myTiles[0];
        int moveRange = owningCharacter.characterDefinition.movement;

        MovementController.PathfindingRules rules = new MovementController.PathfindingRules();
        rules.allowedToPathThroughAllies = true;

        Dictionary<Character, List<Tile>> bestPathForHero = new Dictionary<Character, List<Tile>>();

        HashSet<Character> heroesWithAdjacentAllies = new HashSet<Character>();
        foreach (Character hero in BattleController.Instance.heroes)
        {
            foreach (Character ally in BattleController.Instance.enemies)
            {
                if (ally == owningCharacter || !ally.alive || ally.gameObject.GetComponent<Downed>() != null)
                {
                    continue;
                }
                if (TileGrid.AreCharactersAdjacent(hero, ally))
                {
                    heroesWithAdjacentAllies.Add(hero);
                    break;
                }
            }
        }

        //if we already moved away, we are not allowed to move a second time,
        //so the only candidate tile is the one we are standing on
        List<Tile> candidates;
        if (moved)
        {
            candidates = new List<Tile>() { origin };
        }
        else
        {
            candidates = TileGrid.Instance.GetAllTilesInRange(origin, moveRange);
            if (!candidates.Contains(origin))
            {
                candidates.Add(origin);
            }
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

                    if (blocked)
                    {
                        continue;
                    }

                    if (!bestPathForHero.ContainsKey(hero) || path.Count < bestPathForHero[hero].Count)
                    {
                        bestPathForHero[hero] = path;
                    }
                }
            }
        }

        List<Character> targetPool = new List<Character>();
        foreach (Character hero in bestPathForHero.Keys)
        {
            if (heroesWithAdjacentAllies.Contains(hero))
            {
                targetPool.Add(hero);
            }
        }

        if (targetPool.Count == 0)
        {
            targetPool.AddRange(bestPathForHero.Keys);
        }

        if (targetPool.Count == 0)
        {
            Finish();
            return;
        }

        Util.Shuffle(targetPool);
        targetHero = targetPool[0];
        movePath = bestPathForHero[targetHero];
        litRouteTiles = Util.ExpandPathTiles(movePath, owningCharacter);

        if (movePath.Count == 0)
        {
            ShowAttack();
        }
        else
        {
            AnimationController.Instance.ShowTiles(litRouteTiles, Tile.OverlayType.PossibleMovement, Route, ShowAttack);
        }
    }

    void Route()
    {
        TileGrid.Instance.RouteAICharacterToTile(owningCharacter, new List<Tile>(movePath), ShowAttack);
    }

    void ShowAttack()
    {
        foreach (Tile t in litRouteTiles)
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

        AnimationController.Instance.ScrollToCharacter(targetHero, () =>
        {
            AnimationController.Instance.ShowTiles(attackLine, Tile.OverlayType.PossibleAttck, FireShot);
        }, .5f);
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
		instructions.Add(new CardInstruction("If an enemy is within 3 tiles, move to the tile farthest from all enemies"));
		instructions.Add(new CardInstruction("Target a random hero with an adjacent ally that a line can be drawn to; if none exists, target a random hero a line can be drawn to"));
		instructions.Add(new CardInstruction("Move to the closest tile with line of sight to the target"));
        instructions.Add(new CardInstruction("Deal 10 damage to the first hero in the line"));
        DisplayGrid.Instance.Show();
        return instructions;
    }
}
