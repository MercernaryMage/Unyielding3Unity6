using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Fly : Card
{
	Character target;

	public override void Execute()
	{
		owningCharacter.AddStatusEffect(Type.GetType("Dodging"), null);
		target = FindLowestHPHero();
		if (target == null)
		{
			Finish();
			return;
		}

		Tile destination = FindClosestFitTile(target);
		if (destination != null)
		{
			TileGrid.Instance.MoveCharacterToTile(owningCharacter, destination);
		}

		owningCharacter.SetFacing(TileGrid.Instance.GetFacingDirection(owningCharacter, target));

		List<Tile> attackTiles = TileGrid.Instance.FindCharacter(target);
		AnimationController.Instance.ShowTiles(attackTiles, Tile.OverlayType.PossibleAttck, ReturnFromShowingAttackTiles);
	}

	void ReturnFromShowingAttackTiles()
	{
		ActionController.Instance.PlayAttackAnimation(owningCharacter, null, () =>
		{
			if (TileGrid.AreCharactersAdjacent(owningCharacter, target))
			{
				ActionController.Instance.AttackCharacter(target, owningCharacter, new ActionController.AttackProfile(1, 6, 0));
			}

			foreach (Tile t in TileGrid.Instance.FindCharacter(target))
			{
				t.HideOverlay(Tile.OverlayType.PossibleAttck);
			}

			Finish();
		});
	}

	Character FindLowestHPHero()
	{
		Character lowestHero = null;
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
			if (lowestHero == null || hero.currentHP < lowestHero.currentHP)
			{
				lowestHero = hero;
			}
		}
		return lowestHero;
	}

	Tile FindClosestFitTile(Character targetCharacter)
	{
		return TileGrid.Instance.tiles
			.Where(t => TileGrid.Instance.WouldCharacterFitAtTile(owningCharacter, t))
			.OrderBy(t => TileGrid.Instance.GetDistanceBetweenCharacterAndTile(targetCharacter, t))
			.FirstOrDefault();
	}

	public static List<CardInstruction> GetCardInstructions(CardScriptableObject scriptableObject)
	{
		DisplayGrid.Instance.Clear(11, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction("Teleport adjacent to the hero with the lowest HP"));
		instructions.Add(new CardInstruction());
		instructions.Add(new CardInstruction("If adjacent, deal 1d6 damage"));
		DisplayGrid.Instance.Show();
		return instructions;
	}
}
