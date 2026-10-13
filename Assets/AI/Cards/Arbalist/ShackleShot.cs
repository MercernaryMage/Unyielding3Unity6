using System.Collections.Generic;
using UnityEngine;

public class StakeShot : Card
{
    Character target;
    List<Tile> targetTiles;

    public override void Execute()
    {
        MoveAwayIfNeeded(ReturnFromMoveAway);
    }

    void ReturnFromMoveAway(bool moved)
    {
        List<Character> candidates = new List<Character>();
        foreach (Character hero in BattleController.Instance.heroes)
        {
            if (!hero.alive || hero.GetComponent<Downed>() != null)
            {
                continue;
            }
            if (TileGrid.Instance.CharactersAreAdjacent(owningCharacter, hero))
            {
                continue;
            }
            if (!TileGrid.Instance.DoesCharacterHaveLOSToCharacter(owningCharacter, hero))
            {
                continue;
            }
            candidates.Add(hero);
        }

		if (candidates.Count == 0)
        {
			Finish();
			return;
        }

        target = candidates[Random.Range(0, candidates.Count)];
        targetTiles = TileGrid.Instance.FindCharacter(target);
        AnimationController.Instance.ScrollToCharacter(target, () =>
        {
            AnimationController.Instance.ShowTiles(targetTiles, Tile.OverlayType.PossibleAttck, ReturnFromShowingAttackTiles);
        }, .5f);
    }

    void ReturnFromShowingAttackTiles()
    {
        owningCharacter.SetFacing(TileGrid.Instance.GetFacingDirection(owningCharacter, target));
        ActionController.Instance.PlayAttackAnimation(owningCharacter, null, () =>
        {
            foreach (Tile t in targetTiles)
            {
                t.HideOverlay(Tile.OverlayType.PossibleAttck);
            }

            ActionController.AttackResults results = ActionController.Instance.AttackCharacter(
                target, owningCharacter, new ActionController.AttackProfile(1, 6, 0, true));
            if (results.hit)
            {
                target.AddStatusEffect(typeof(Paralyzed), null);
                target.AddStatusEffect(typeof(Staked), null);
            }

            Finish();
        });
    }

    public static List<CardInstruction> GetCardInstructions(CardScriptableObject scriptableObject)
    {
        DisplayGrid.Instance.Clear(11, 8);
        List<CardInstruction> instructions = new List<CardInstruction>();
        instructions.Add(new CardInstruction("Move away from near by enemies"));
        instructions.Add(new CardInstruction("Attack a non-adjacent hero in line of sight for 1d6 damage"));
        instructions.Add(new CardInstruction("On hit, apply <u>Paralyzed</u> and <u>Staked</u>"));
        DisplayGrid.Instance.Show();
        return instructions;
    }
}
