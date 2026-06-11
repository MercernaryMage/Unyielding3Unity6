using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainWhirl : Card
{
	List<Character> heroesInRange;
	List<Tile> hitTiles;

	public override void Execute()
	{
		heroesInRange = GetHeroesInRange(3);

		if (heroesInRange.Count == 0)
		{
			FloatingCombatNumberController.Instance.QueueFloatingCombatNumber(owningCharacter, "No target");
			Util.BringCardToTopOfDeck(owningCharacter, "HookAndPull");
			AnimationController.Instance.DelayedCallback(3f, () => AIController.Instance.TakeTurn(owningCharacter));
			return;
		}

		hitTiles = new List<Tile>();
		foreach (Character hero in heroesInRange)
		{
			foreach (Tile t in TileGrid.Instance.FindCharacter(hero))
			{
				if (!hitTiles.Contains(t))
				{
					hitTiles.Add(t);
				}
			}
		}

		AnimationController.Instance.ShowTiles(hitTiles, Tile.OverlayType.PossibleAttck, ReturnFromShowingAttackTiles);
	}

	List<Character> GetHeroesInRange(int range)
	{
		List<Tile> myTiles = TileGrid.Instance.FindCharacter(owningCharacter);
		List<Character> result = new List<Character>();

		foreach (Character hero in BattleController.Instance.heroes)
		{
			if (!hero.alive)
			{
				continue;
			}
			if (hero.gameObject.GetComponent<Downed>() != null)
			{
				continue;
			}

			List<Tile> heroTiles = TileGrid.Instance.FindCharacter(hero);
			int minDist = int.MaxValue;
			foreach (Tile myTile in myTiles)
			{
				foreach (Tile heroTile in heroTiles)
				{
					int dist = TileGrid.Distance(myTile, heroTile);
					if (dist < minDist)
					{
						minDist = dist;
					}
				}
			}

			if (minDist <= range)
			{
				result.Add(hero);
			}
		}

		return result;
	}

	public void ReturnFromShowingAttackTiles()
	{
		ActionController.Instance.PlayAttackAnimation(owningCharacter, null, () =>
		{
			foreach (Character hero in heroesInRange)
			{
				ActionController.AttackResults results = new ActionController.AttackResults();
				results.fakeHit = true;
				ActionController.Instance.DamageCharacter(hero, owningCharacter, new ActionController.AttackProfile(0, 0, 4), results);
			}

			foreach (Tile t in hitTiles)
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
		instructions.Add(new CardInstruction("If any enemies are within range 3,"));
		instructions.Add(new CardInstruction("deal 4 damage to all of them"));
		instructions.Add(new CardInstruction("Otherwise, play a Hook'N'Pull card"));
		DisplayGrid.Instance.Show();

		return instructions;
	}
}
