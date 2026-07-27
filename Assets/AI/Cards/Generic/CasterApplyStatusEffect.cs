using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CasterApplyStatusEffect : Card
{
    Tuple<List<Tile>, Tile> route;
    List<Tile> litRouteTiles;
    Character targetHero;

	public override void Execute()
	{
		MoveAwayIfNeeded(FinishedRunning);
	}

	public void FinishedRunning(bool moved)
    {
        List<Character> heroesInRange = Util.GetHeroesInRange(owningCharacter, 5);
        heroesInRange.RemoveAll(o => !TileGrid.Instance.DoesCharacterHaveLOSToCharacter(o, owningCharacter));
        if (heroesInRange.Count == 0)
        {
            //if we already moved away, we are not allowed to move a second time
            if (moved)
            {
                Finish();
                return;
            }
            DoMove();
        }
        else
        {
            targetHero = GetTarget();
			if (targetHero == null)
			{
				Finish();
				return;
			}
			AnimationController.Instance.ScrollToCharacter(targetHero, ApplyDoom, .5f);
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

        litRouteTiles = Util.ExpandPathTiles(route.Item1, owningCharacter);
        AnimationController.Instance.ShowTiles(litRouteTiles, Tile.OverlayType.PossibleMovement, ReturnFromShowingTiles, ReturnFromMove);
    }

    void ReturnFromShowingTiles()
    {
        TileGrid.Instance.RouteAICharacterToTile(owningCharacter, new List<Tile>(route.Item1), ReturnFromMove);
    }

    void ReturnFromMove()
    {
        foreach (Tile t in litRouteTiles)
        {
            t.HideOverlay(Tile.OverlayType.PossibleMovement);
        }



        targetHero = GetTarget();
		if (targetHero == null)
        {
			Finish();
			return;
		}
		AnimationController.Instance.ScrollToCharacter(targetHero, ApplyDoom, .5f);
    }

    void ApplyDoom()
    {
		targetHero.AddStatusEffect(Type.GetType(GetStringValue("StatusEffectName")), null);
        AnimationController.Instance.DelayedCallback(1.0f, () => Finish());
    }

	Character GetTarget()
	{
		List<Character> heroesInRange = Util.GetHeroesInRange(owningCharacter, 5);
		heroesInRange.RemoveAll(o => !TileGrid.Instance.DoesCharacterHaveLOSToCharacter(o, owningCharacter));
		if (heroesInRange.Count == 0)
		{
			return null;
		}

		return heroesInRange[UnityEngine.Random.Range(0, heroesInRange.Count)];
	}

	public static List<CardInstruction> GetCardInstructions(CardScriptableObject scriptableObject)
    {
        DisplayGrid.Instance.Clear(11, 8);
        List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction("If an enemy is within 3 tiles, move to the tile farthest from all enemies"));
		instructions.Add(new CardInstruction("If no enemy is within range 5,"));
        instructions.Add(new CardInstruction("move toward closest enemy, stopping 3 away"));
        instructions.Add(new CardInstruction($"Apply <u>{scriptableObject.GetTagStringValue("StatusEffectDisplayName")}</u> to a random enemy in range 5"));
        DisplayGrid.Instance.Show();
        return instructions;
    }
}
