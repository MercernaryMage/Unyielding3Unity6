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

		//Telegraph: mark every tile within range 2 of the target, then queue the blast to
		//go off as a turn event (fires between character turns on the given tick).
		List<Tile> outlineTiles = GetTilesAroundCharacter(target, 2);
		int outlineId = OutlineManager.Instance.Create(outlineTiles, Util.HexToColor("802080"));

		//Fire on the caster's own tick, so the blast goes off just before its next turn.
		int detonationTick = TurnControl.Instance.GetValue(owningCharacter);
		TurnEventController.Instance.AddEvent(() => Detonate(outlineTiles, outlineId), detonationTick);

		AnimationController.Instance.ScrollToCharacter(target, () => Finish(), .5f);
	}

	//Hit every character standing on a marked tile once, then clear the telegraph outline.
	void Detonate(List<Tile> tiles, int outlineId)
	{
		List<Character> hitCharacters = new List<Character>();
		foreach (Tile t in tiles)
		{
			if (t.character != null && !hitCharacters.Contains(t.character))
			{
				hitCharacters.Add(t.character);
			}
		}

		foreach (Character c in hitCharacters)
		{
			ActionController.Instance.AttackCharacter(c, owningCharacter, new ActionController.AttackProfile(1, 6, 3));
		}

		OutlineManager.Instance.Destroy(outlineId);
	}

	Character GetLowestMaxHPTarget()
	{
		//Prefer the lowest max HP enemy we haven't targeted yet. Once every enemy has
		//been targeted, clear the history and cycle through them again.
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

	List<Tile> GetTilesAroundCharacter(Character c, int range)
	{
		List<Tile> result = new List<Tile>();
		foreach (Tile characterTile in TileGrid.Instance.FindCharacter(c))
		{
			foreach (Tile t in TileGrid.Instance.GetAllTilesInRange(characterTile, range))
			{
				if (!result.Contains(t))
				{
					result.Add(t);
				}
			}
		}
		return result;
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
