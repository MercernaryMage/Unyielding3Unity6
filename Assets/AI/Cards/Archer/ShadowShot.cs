using System.Collections.Generic;
using UnityEngine;

public class ShadowShot : Card
{
    Character target;

    public override void Execute()
    {
        MoveAwayIfNeeded(ReturnFromMoveAway);
    }

    void ReturnFromMoveAway(bool moved)
    {
        PickShadowTarget();
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

    void PickShadowTarget()
    {
        List<Character> livingHeroes = GetLivingEnemies();
        if (livingHeroes.Count == 0)
        {
            Finish();
            return;
        }

        //prefer heroes the caster cannot see; only fall back to visible heroes if all are in line of sight
        List<Character> outOfSight = new List<Character>();
        foreach (Character hero in livingHeroes)
        {
            if (!TileGrid.Instance.DoesCharacterHaveLOSToCharacter(owningCharacter, hero))
            {
                outOfSight.Add(hero);
            }
        }

        List<Character> pool = outOfSight.Count > 0 ? outOfSight : livingHeroes;
        target = pool[Random.Range(0, pool.Count)];
        AnimationController.Instance.ScrollToCharacter(target, Delay, .5f);
    }

    void Delay()
    {
        Shadowed shadowed = (Shadowed)target.AddStatusEffect(typeof(Shadowed), null);
        shadowed.causingCharacter = owningCharacter;
        Finish();
    }

    public static List<CardInstruction> GetCardInstructions(CardScriptableObject scriptableObject)
    {
        DisplayGrid.Instance.Clear(11, 8);
        List<CardInstruction> instructions = new List<CardInstruction>();
        instructions.Add(new CardInstruction("If an enemy is within 3 tiles, move to the tile farthest from all enemies"));
        instructions.Add(new CardInstruction("Apply Shadowed to a random enemy, preferring one not in line of sight"));
        return instructions;
    }
}
