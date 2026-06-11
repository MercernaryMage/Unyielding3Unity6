using System.Collections.Generic;
using UnityEngine;

public class LeapOut : Card
{
	Character target;

	public override void Execute()
	{
		target = null;
		int farthestDist = -1;

		foreach (Character hero in BattleController.Instance.heroes)
		{
			if (!hero.alive)
			{
				continue;
			}
			if (hero.GetComponent<Downed>() != null)
			{
				continue;
			}
			int dist = TileGrid.Instance.GetDistanceBetweenCharacters(owningCharacter, hero);
			if (dist > farthestDist)
			{
				farthestDist = dist;
				target = hero;
			}
		}

		if (target == null)
		{
			Finish();
			return;
		}

		Tile landingTile = null;
		List<Tile> adjacentTiles = TileGrid.Instance.GetAllAdjacentTilesToCharacter(target);
		foreach (Tile t in adjacentTiles)
		{
			if (TileGrid.Instance.WouldCharacterFitAtTile(owningCharacter, t))
			{
				landingTile = t;
				break;
			}
		}

		if (landingTile == null)
		{
			Finish();
			return;
		}

		List<Tile> targetTiles = TileGrid.Instance.FindCharacter(target);
		AnimationController.Instance.ShowTiles(targetTiles, Tile.OverlayType.PossibleAttck, () =>
		{
			foreach (Tile t in targetTiles)
			{
				t.HideOverlay(Tile.OverlayType.PossibleAttck);
			}
			owningCharacter.SetFacing(TileGrid.Instance.GetFacingDirection(owningCharacter, target));
			TileGrid.Instance.MoveCharacterToTile(owningCharacter, landingTile);
			target.AddStatusEffect(typeof(Paralyzed), null);
			Finish();
		});
	}

	public static List<CardInstruction> GetCardInstructions(CardScriptableObject scriptableObject)
	{
		DisplayGrid.Instance.Clear(11, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction("Teleport adjacent to the farthest enemy"));
		instructions.Add(new CardInstruction("Apply Paralyzed to them"));
		DisplayGrid.Instance.Show();
		return instructions;
	}
}
