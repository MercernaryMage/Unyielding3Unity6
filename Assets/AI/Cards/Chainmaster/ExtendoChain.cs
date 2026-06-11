using System;
using System.Collections.Generic;
using UnityEngine;
using static ActionController;

public class ExtendoChain : Card
{
	TemplateLibrary.TilesAndDirection tilesAndDirection;
	Character targetHero;

	public override void Execute()
	{
		targetHero = null;
		int lowestDefense = int.MaxValue;

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

			int defense = hero.toughness + hero.armor;
			if (defense < lowestDefense)
			{
				lowestDefense = defense;
				targetHero = hero;
			}
		}

		if (targetHero == null)
		{
			Debug.Log("No targets");
			Finish();
			return;
		}

		List<Tile> tiles = TileGrid.Instance.FindCharacter(targetHero);
		tilesAndDirection = new TemplateLibrary.TilesAndDirection(tiles, Direction.East);
		AnimationController.Instance.ShowTiles(tiles, Tile.OverlayType.PossibleAttck, ReturnFromShowingAttackTiles);
	}

	public void ReturnFromShowingAttackTiles()
	{
		owningCharacter.SetFacing(TileGrid.Instance.GetFacingDirection(owningCharacter, targetHero));
		ActionController.Instance.PlayAttackAnimation(owningCharacter, null, () =>
		{
			AttackResults results = ActionController.Instance.AttackCharacter(targetHero, owningCharacter, new ActionController.AttackProfile(1, 6, 0));

			foreach (Tile t in tilesAndDirection.tiles)
			{
				t.HideOverlay(Tile.OverlayType.PossibleAttck);
			}

			if (results.hit && targetHero.alive)
			{
				Util.PullToAttacker(targetHero, owningCharacter);
			}

			Finish();
		});
	}

	public static List<CardInstruction> GetCardInstructions(CardScriptableObject scriptableObject)
	{
		DisplayGrid.Instance.Clear(11, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction("Attack the enemy with lowest toughness + armor"));
		instructions.Add(new CardInstruction("Range: infinite"));
		instructions.Add(new CardInstruction("Deal 1d6 damage"));
		instructions.Add(new CardInstruction("If target survives, pull them adjacent"));
		DisplayGrid.Instance.Show();

		return instructions;
	}
}
