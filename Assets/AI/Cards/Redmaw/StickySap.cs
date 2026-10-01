using System;
using System.Collections.Generic;
using UnityEngine;

public class StickySap : Card
{
	Character target;
	List<Tile> attackTiles;

	public override void Execute()
	{
		target = GetTarget();
		if (target == null)
		{
			Debug.Log("No target");
			Finish();
			return;
		}

		AnimationController.Instance.ScrollToCharacter(target, ShowAttackTiles, .5f);
	}

	Character GetTarget()
	{
		Character farthest = null;
		int farthestDistance = -1;

		foreach (Character hero in BattleController.Instance.heroes)
		{
			if (!hero.alive || hero.IsDowned())
			{
				continue;
			}
			if (!TileGrid.Instance.DoesCharacterHaveLOSToCharacter(hero, owningCharacter))
			{
				continue;
			}

			int distance = TileGrid.Instance.GetDistanceBetweenCharacters(owningCharacter, hero);
			if (distance > farthestDistance)
			{
				farthestDistance = distance;
				farthest = hero;
			}
		}

		return farthest;
	}

	void ShowAttackTiles()
	{
		attackTiles = new List<Tile>(TileGrid.Instance.FindCharacter(target));
		foreach (Tile t in TileGrid.Instance.GetSurroundingTiles(target))
		{
			if (t != null && !attackTiles.Contains(t))
			{
				attackTiles.Add(t);
			}
		}

		AnimationController.Instance.ShowTiles(attackTiles, Tile.OverlayType.PossibleAttck, Attack);
	}

	void Attack()
	{
		owningCharacter.SetFacing(TileGrid.Instance.GetFacingDirection(owningCharacter, target));
		ActionController.Instance.PlayAttackAnimation(owningCharacter, null, () =>
		{
			foreach (Character c in GetTargetsOnTiles(attackTiles))
			{
				if (!c.hero)
				{
					continue;
				}
				ActionController.Instance.AttackCharacter(c, owningCharacter, new ActionController.AttackProfile(1, 6, 0, true));
				c.AddStatusEffect(typeof(Sapped), null);
			}

			foreach (Tile t in attackTiles)
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
		instructions.Add(new CardInstruction("Target the farthest enemy in line of sight"));
		instructions.Add(new CardInstruction("Deal 1d6 damage to every enemy in the 3x3 around them"));
		instructions.Add(new CardInstruction("and apply <u>Sapped</u>"));

		DisplayGrid.Instance.Add(DisplayGrid.DisplayGridObject.Size1Enemy, DisplayGrid.DisplayGridDirection.South, 5, 4);
		DisplayGrid.Instance.Add(DisplayGrid.DisplayGridObject.EffectedTile, new List<Tuple<int, int>>()
		{
			new Tuple<int, int>(4, 5),
			new Tuple<int, int>(5, 5),
			new Tuple<int, int>(6, 5),
			new Tuple<int, int>(4, 4),
			new Tuple<int, int>(6, 4),
			new Tuple<int, int>(4, 3),
			new Tuple<int, int>(5, 3),
			new Tuple<int, int>(6, 3),
		});
		DisplayGrid.Instance.Show();

		return instructions;
	}
}
