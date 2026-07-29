using System.Collections.Generic;
using UnityEngine;

public class Blast : Card
{
	List<Character> alreadyTargeted = new List<Character>();

	public override void Execute()
	{
		Character target = GetLowestMaxHPTarget();
		if (target == null)
		{
			Debug.Log("No target");
			Finish();
			return;
		}

		List<Tile> warnedTiles = TileGrid.Instance.GetAllTilesInRangeOfCharacter(target, 2);
		foreach (Tile t in warnedTiles)
		{
			t.AddWarning();
		}

		Tile centerTile = TileGrid.Instance.FindCharacter(target)[0];
		int detonationTick = TurnControl.Instance.GetValue(owningCharacter);
		TurnEventController.Instance.AddEvent(() => Detonate(centerTile, warnedTiles), detonationTick);

		AnimationController.Instance.ScrollToCharacter(target, () => Finish(), .5f);
	}

	void Detonate(Tile centerTile, List<Tile> warnedTiles)
	{
		AnimationController.Instance.ScrollToTile(centerTile, () => DetonateActual(centerTile, warnedTiles), .5f);
	}

	void DetonateActual(Tile centerTile, List<Tile> warnedTiles)
	{
		ClearWarnings(warnedTiles);

		List<Character> hitCharacters = new List<Character>();
		foreach (Tile t in warnedTiles)
		{
			if (t.character != null && !hitCharacters.Contains(t.character))
			{
				hitCharacters.Add(t.character);
			}
		}

		NoFriendlyFire noFriendlyFire = owningCharacter.gameObject.GetComponent<NoFriendlyFire>();
		if (noFriendlyFire != null)
		{
			noFriendlyFire.SpareAllies(hitCharacters);
		}

		if (hitCharacters.Count == 0)
		{
			FloatingCombatNumberController.Instance.ShowFloatingCombatNumber(
				centerTile.transform.position + Vector3.up * 1.5f, "no target");
		}

		foreach (Character c in hitCharacters)
		{
			ActionController.Instance.AttackCharacter(c, owningCharacter, new ActionController.AttackProfile(1, 6, 3));
		}

		TurnEventController.Instance.Pump();
	}

	void ClearWarnings(List<Tile> warnedTiles)
	{
		foreach (Tile t in warnedTiles)
		{
			t.RemoveWarning();
		}
	}

	public Character GetLowestMaxHPTarget()
	{
		Character target = FindLowestUntargeted();
		if (target == null)
		{
			alreadyTargeted.Clear();
			target = FindLowestUntargeted();
		}
		if (target != null)
		{
			alreadyTargeted.Add(target);
		}
		return target;
	}

	Character FindLowestUntargeted()
	{
		Character lowest = null;
		foreach (Character hero in BattleController.Instance.heroes)
		{
			if (!hero.alive || hero.GetComponent<Downed>() != null)
			{
				continue;
			}
			if (alreadyTargeted.Contains(hero))
			{
				continue;
			}
			if (lowest == null || hero.maxHP < lowest.maxHP)
			{
				lowest = hero;
			}
		}
		return lowest;
	}

	public static List<CardInstruction> GetCardInstructions(CardScriptableObject scriptableObject)
	{
		DisplayGrid.Instance.Clear(11, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction("Target the enemy with the lowest max HP"));
		instructions.Add(new CardInstruction("Mark the tiles within range 2 of the target"));
		instructions.Add(new CardInstruction("At the start of your next turn, deal 1d6+3 damage to any character on a marked tile"));
		DisplayGrid.Instance.Show();

		return instructions;
	}
}
