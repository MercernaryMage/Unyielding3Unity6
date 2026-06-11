using System;
using System.Collections.Generic;
using UnityEngine;

public class FightItOut : Card
{
    int range = 3;
    Character attackTarget;
    List<Tile> attackTiles;
    Tuple<List<Tile>, Tile> moveRoute;
    Character followUpTarget;
    List<Tile> followUpAttackTiles;

    public override void Execute()
    {
        attackTarget = null;
        int closestDistance = int.MaxValue;

        foreach (Character hero in BattleController.Instance.heroes)
        {
            if (!hero.alive || hero.GetComponent<Downed>() != null)
            {
                continue;
            }

            int dist = TileGrid.Instance.GetDistanceBetweenCharacters(owningCharacter, hero);
            if (dist <= range && dist < closestDistance)
            {
                closestDistance = dist;
                attackTarget = hero;
            }
        }

        if (attackTarget == null)
        {
            DoMove();
            return;
        }

        attackTiles = TileGrid.Instance.FindCharacter(attackTarget);
        owningCharacter.SetFacing(TileGrid.Instance.GetFacingDirection(owningCharacter, attackTarget));
        AnimationController.Instance.ShowTiles(attackTiles, Tile.OverlayType.PossibleAttck, ReturnFromShowingAttackTiles);
    }

    void ReturnFromShowingAttackTiles()
    {
        ActionController.Instance.PlayAttackAnimation(owningCharacter, null, () =>
        {
            foreach (Tile t in attackTiles)
            {
                if (t.character != null && t.character.hero)
                {
                    ActionController.Instance.AttackCharacter(t.character, owningCharacter, new ActionController.AttackProfile(1, 3, 0));
                }
                t.HideOverlay(Tile.OverlayType.PossibleAttck);
            }

            DoMove();
        });
    }

    void DoMove()
    {
        if (TemplateLibrary.GetAdjacentCharacterTarget(owningCharacter, null) != null)
        {
            Finish();
            return;
        }

        Dictionary<Character, Tuple<List<Tile>, Tile>> routes = RouteToAllClosestCharacters(true);
        moveRoute = Util.FindSmallestRoute(routes, null);

        if (moveRoute == null)
        {
            Finish();
            return;
        }

        Util.ShortenPathToMaxRange(moveRoute, owningCharacter.characterDefinition.movement + 1);
        AnimationController.Instance.ShowTiles(moveRoute.Item1, Tile.OverlayType.PossibleMovement, ReturnFromShowingMoveTiles, ReturnFromMove);
    }

    void ReturnFromShowingMoveTiles()
    {
        TileGrid.Instance.RouteCharacterToTile(owningCharacter, new List<Tile>(moveRoute.Item1), ReturnFromMove);
    }

    void ReturnFromMove()
    {
        foreach (Tile t in moveRoute.Item1)
        {
            t.HideOverlay(Tile.OverlayType.PossibleMovement);
        }

        followUpTarget = null;
        foreach (Tile t in TileGrid.Instance.GetAllAdjacentTilesToCharacter(owningCharacter))
        {
            if (t.character == null || !t.character.hero || t.character.IsDowned())
            {
                continue;
            }

            if (followUpTarget == null || t.character.currentHP < followUpTarget.currentHP)
            {
                followUpTarget = t.character;
            }
        }

        if (followUpTarget == null)
        {
            Finish();
            return;
        }

        followUpAttackTiles = TileGrid.Instance.FindCharacter(followUpTarget);
        owningCharacter.SetFacing(TileGrid.Instance.GetFacingDirection(owningCharacter, followUpTarget));
        AnimationController.Instance.ShowTiles(followUpAttackTiles, Tile.OverlayType.PossibleAttck, ReturnFromShowingFollowUpAttackTiles);
    }

    void ReturnFromShowingFollowUpAttackTiles()
    {
        ActionController.Instance.PlayAttackAnimation(owningCharacter, null, () =>
        {
            foreach (Tile t in followUpAttackTiles)
            {
                if (t.character != null && t.character.hero)
                {
                    ActionController.Instance.AttackCharacter(t.character, owningCharacter, new ActionController.AttackProfile(1, 3, 0));
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
        instructions.Add(new CardInstruction("Attack closest enemy within range 3"));
        instructions.Add(new CardInstruction("If not adjacent, move toward closest enemy"));
        instructions.Add(new CardInstruction("If now adjacent, attack the enemy with least HP"));
        DisplayGrid.Instance.Show();
        return instructions;
    }
}
