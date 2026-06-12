using System;
using System.Collections.Generic;
using UnityEngine;

public class Roar : Card
{
	List<Character> targets;
	List<Tile> targetTiles;

	public override void Execute()
	{
		targets = new List<Character>();
		foreach (Character hero in BattleController.Instance.heroes)
		{
			if (!hero.alive || hero.IsDowned())
			{
				continue;
			}
			if (TileGrid.AreCharactersAdjacent(owningCharacter, hero))
			{
				continue;
			}
			targets.Add(hero);
		}

		if (targets.Count == 0)
		{
			Finish();
			return;
		}

		targetTiles = new List<Tile>();
		foreach (Character target in targets)
		{
			foreach (Tile t in TileGrid.Instance.FindCharacter(target))
			{
				if (!targetTiles.Contains(t))
				{
					targetTiles.Add(t);
				}
			}
		}

		AnimationController.Instance.ShowTiles(targetTiles, Tile.OverlayType.PossibleAttck, ReturnFromShowingAttackTiles);
	}

	void ReturnFromShowingAttackTiles()
	{
		ActionController.Instance.PlayAttackAnimation(owningCharacter, null, () =>
		{
			foreach (Character target in targets)
			{
				target.AddStatusEffect(typeof(Weakened), null);
			}
			foreach (Tile t in targetTiles)
			{
				t.HideOverlay(Tile.OverlayType.PossibleAttck);
			}
			Finish();
		});
	}

	public static List<CardInstruction> GetCardInstructions(CardScriptableObject scriptableObject)
	{
		DisplayGrid.Instance.Clear(11, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction("Apply Weakened to all non-adjacent enemies"));
		DisplayGrid.Instance.Show();
		return instructions;
	}
}
