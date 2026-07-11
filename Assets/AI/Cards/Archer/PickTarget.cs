using System.Collections.Generic;
using UnityEngine;

public class PickTarget : Card
{
    Character target;

    public override void Execute()
    {
        MoveAwayIfNeeded(PickLockOnTarget);
    }

    void PickLockOnTarget()
    {
        List<Character> candidates = new List<Character>();
        foreach (Character hero in BattleController.Instance.heroes)
        {
            if (!hero.alive || hero.GetComponent<Downed>() != null)
            {
                continue;
            }
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
        instructions.Add(new CardInstruction("If an enemy is within 3 tiles, move to the tile farthest from them"));
        instructions.Add(new CardInstruction("Apply Locked On to a random enemy in line of sight"));
        return instructions;
    }
}
