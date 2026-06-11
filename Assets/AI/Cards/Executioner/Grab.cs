using System.Collections.Generic;
using UnityEngine;
using static ActionController;

public class Grab : Card
{
	Character target;
	List<Tile> targetTiles;

	public override void Execute()
	{
		List<Tile> myTiles = TileGrid.Instance.FindCharacter(owningCharacter);
		target = null;
		int closestDist = int.MaxValue;

		foreach (Character hero in BattleController.Instance.heroes)
		{
			if (!hero.alive)
			{
				continue;
			}
			if (hero.GetComponent<Downed>())
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

			if (minDist <= 2 && minDist < closestDist)
			{
				closestDist = minDist;
				target = hero;
			}
		}

		if (target == null)
		{
			Finish();
			return;
		}

		targetTiles = TileGrid.Instance.FindCharacter(target);
		AnimationController.Instance.ShowTiles(targetTiles, Tile.OverlayType.PossibleAttck, ReturnFromShowingAttackTiles);
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

			if (target.alive)
			{
				Util.PullToAttacker(target, owningCharacter);
				if (TileGrid.Instance.CharactersAreAdjacent(owningCharacter, target))
				{
					Util.BringCardToTopOfDeck(owningCharacter, "Guillotine");
					AIController.Instance.TakeTurn(owningCharacter);
					return;
				}
			}

			Finish();
		});
	}

	public static List<CardInstruction> GetCardInstructions(CardScriptableObject scriptableObject)
	{
		DisplayGrid.Instance.Clear(11, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction("Pull an enemy within range 2 adjacent"));
		instructions.Add(new CardInstruction("If adjacent after pull, draw and play Chop"));
		return instructions;
	}
}
