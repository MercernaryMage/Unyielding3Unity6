using System;
using System.Collections.Generic;
using UnityEngine;

public class PowerShot : Card
{
	List<Tile> warnedTiles;
	bool running;

	public override void Execute()
	{
		List<Tile> ownTiles = TileGrid.Instance.FindCharacter(owningCharacter);
		Tile origin = ownTiles[0];

		List<List<Tile>> bestLines = new List<List<Tile>>();
		int bestCount = 0;

		foreach (Tile endTile in TileGrid.Instance.tiles)
		{
			if (endTile == origin)
			{
				continue;
			}

			List<Tile> line = TileGrid.Instance.GetLineTilesTillCollision(origin, endTile);
			foreach (Tile ownTile in ownTiles)
			{
				line.Remove(ownTile);
			}

			List<Character> hitHeroes = new List<Character>();
			foreach (Tile t in line)
			{
				if (t.character == null || !t.character.hero || !t.character.alive || t.character.IsDowned())
				{
					continue;
				}
				if (!hitHeroes.Contains(t.character))
				{
					hitHeroes.Add(t.character);
				}
			}

			if (hitHeroes.Count == 0 || hitHeroes.Count < bestCount)
			{
				continue;
			}

			if (hitHeroes.Count > bestCount)
			{
				bestCount = hitHeroes.Count;
				bestLines.Clear();
			}
			bestLines.Add(line);
		}

		if (bestLines.Count == 0)
		{
			ShowNoTarget(owningCharacter.token.transform.position);
			Finish();
			return;
		}

		warnedTiles = bestLines[UnityEngine.Random.Range(0, bestLines.Count)];

		owningCharacter.SetFacing(TileGrid.Instance.GetFacingDirection(origin, warnedTiles[warnedTiles.Count - 1]));
		TileGrid.AddWarnings(warnedTiles);

		running = true;

		int shotTick = TurnControl.Instance.GetValue(owningCharacter);
		TurnEventController.Instance.AddEvent(FireShot, shotTick);

		Finish();
	}

	void Cancel()
	{
		if (!running)
		{
			return;
		}

		running = false;
		TileGrid.RemoveWarnings(warnedTiles);
	}

	void FireShot()
	{
		if (!running)
		{
			TurnEventController.Instance.Pump();
			return;
		}

		if (!owningCharacter.alive)
		{
			Cancel();
			TurnEventController.Instance.Pump();
			return;
		}

		AnimationController.Instance.ScrollToCharacter(owningCharacter, FireShotActual, .5f);
	}

	void FireShotActual()
	{
		running = false;
		TileGrid.RemoveWarnings(warnedTiles);

		List<Character> hitCharacters = GetTargetsOnTiles(warnedTiles);

		ActionController.Instance.PlayAttackAnimation(owningCharacter, null, () =>
		{
			if (hitCharacters.Count == 0)
			{
				ShowNoTarget(owningCharacter.token.transform.position);
			}

			AttackCharacters(hitCharacters, new ActionController.AttackProfile(1, 6, 0, true));

			TurnEventController.Instance.Pump();
		});
	}

	public override void OnCharacterKnockbackFinished(CharacterKnockbackFinishMessage message)
	{
		if (message.knockedBackCharacter == owningCharacter)
		{
			Cancel();
		}
	}

	public override void OnCharacterDied(CharacterDiedMessage message)
	{
		if (message.character == owningCharacter)
		{
			Cancel();
		}
	}

	public static List<CardInstruction> GetCardInstructions(CardScriptableObject scriptableObject)
	{
		DisplayGrid.Instance.Clear(11, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction("Mark a line through as many enemies as possible and <u>Charge</u>"));
		instructions.Add(new CardInstruction("On <u>Charge</u> completion: deal 1d6 damage to any character on a marked tile"));

		return instructions;
	}
}
