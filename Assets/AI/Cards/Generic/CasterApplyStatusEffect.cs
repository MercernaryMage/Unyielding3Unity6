using System;
using System.Collections.Generic;
using UnityEngine;

public class CasterApplyStatusEffect : Card
{
    Tuple<List<Tile>, Tile> route;
    Character targetHero;

    public override void Execute()
    {
        List<Character> heroesInRange = Util.GetHeroesInRange(owningCharacter, 5);
        heroesInRange.RemoveAll(o => !TileGrid.Instance.DoesCharacterHaveLOSToCharacter(o, owningCharacter));
        if (heroesInRange.Count == 0)
        {
            DoMove();
        }
        else
        {
            AnimationController.Instance.DelayedCallback(1f, ApplyDoom);
        }
    }

    void DoMove()
    {
        Dictionary<Character, Tuple<List<Tile>, Tile>> routes = RouteToAllClosestCharacters(true);

        route = null;
        targetHero = null;
        foreach (KeyValuePair<Character, Tuple<List<Tile>, Tile>> pair in routes)
        {
            if (!pair.Key.alive)
            {
                continue;
            }
            if (route == null || route.Item1.Count > pair.Value.Item1.Count)
            {
                route = pair.Value;
                targetHero = pair.Key;
            }
        }

        if (route == null)
        {
            return;
        }

        Util.ShortenPathToDesiredRange(route, owningCharacter, targetHero, 3);
        Util.ShortenPathToMaxRange(route, owningCharacter.characterDefinition.movement + 1);

        AnimationController.Instance.ShowTiles(route.Item1, Tile.OverlayType.PossibleMovement, ReturnFromShowingTiles, ReturnFromMove);
    }

    void ReturnFromShowingTiles()
    {
        TileGrid.Instance.RouteCharacterToTile(owningCharacter, new List<Tile>(route.Item1), ReturnFromMove);
    }

    void ReturnFromMove()
    {
        foreach (Tile t in route.Item1)
        {
            t.HideOverlay(Tile.OverlayType.PossibleMovement);
        }
        ApplyDoom();
    }

    void ApplyDoom()
    {
        List<Character> heroesInRange = Util.GetHeroesInRange(owningCharacter, 5);
        heroesInRange.RemoveAll(o => !TileGrid.Instance.DoesCharacterHaveLOSToCharacter(o, owningCharacter));
        if (heroesInRange.Count == 0)
        {
            Finish();
            return;
        }

        Character target = heroesInRange[UnityEngine.Random.Range(0, heroesInRange.Count)];
		SelectionManager.Instance.SnapCameraToCharacter(target);
		target.AddStatusEffect(Type.GetType(GetStringValue("StatusEffectName")), null);
        AnimationController.Instance.DelayedCallback(1.0f, () => Finish());
    }

    public static List<CardInstruction> GetCardInstructions(CardScriptableObject scriptableObject)
    {
        DisplayGrid.Instance.Clear(11, 8);
        List<CardInstruction> instructions = new List<CardInstruction>();
        instructions.Add(new CardInstruction("If no enemy is within range 5,"));
        instructions.Add(new CardInstruction("move toward closest enemy, stopping 3 away"));
        instructions.Add(new CardInstruction($"Apply {scriptableObject.GetTagStringValue("StatusEffectDisplayName")} to a random enemy in range 5"));
        DisplayGrid.Instance.Show();
        return instructions;
    }
}
