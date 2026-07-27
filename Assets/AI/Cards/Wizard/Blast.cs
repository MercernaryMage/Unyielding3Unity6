using System.Collections.Generic;
using UnityEngine;

public class Blast : Card
{
	public override void Execute()
	{
		Character target = GetLowestMaxHPTarget();
		if (target == null)
		{
			Debug.Log("No target");
			Finish();
			return;
		}

		//Telegraph: mark every tile within range 2 of the target and remember it on the caster.
		List<Tile> outlineTiles = GetTilesAroundCharacter(target, 2);
		int outlineId = OutlineManager.Instance.Create(outlineTiles, Util.HexToColor("802080"));

		Incoming incoming = (Incoming)owningCharacter.AddStatusEffect(typeof(Incoming), null);
		incoming.Set(outlineTiles, outlineId);

		AnimationController.Instance.ScrollToCharacter(target, () => Finish(), .5f);
	}

	Character GetLowestMaxHPTarget()
	{
		Character lowest = null;
		foreach (Character hero in BattleController.Instance.heroes)
		{
			if (!hero.alive || hero.GetComponent<Downed>() != null)
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
