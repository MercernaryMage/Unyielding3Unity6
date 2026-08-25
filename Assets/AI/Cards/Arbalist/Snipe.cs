using System.Collections.Generic;
using UnityEngine;

public class Snipe : Card
{
	List<Tile> warnedTiles;
	bool running;

	public override void Execute()
	{
		MoveAwayIfNeeded(ReturnFromMoveAway);
	}

	void ReturnFromMoveAway(bool moved)
	{
		List<Character> engagedHeroes = new List<Character>();
		List<Character> allHeroes = new List<Character>();

		foreach (Character hero in BattleController.Instance.heroes)
		{
			if (!hero.alive || hero.GetComponent<Downed>() != null)
			{
				continue;
			}

			allHeroes.Add(hero);

			foreach (Character ally in BattleController.Instance.enemies)
			{
				if (ally == owningCharacter || !ally.alive || ally.GetComponent<Downed>() != null)
				{
					continue;
				}
				if (TileGrid.AreCharactersAdjacent(hero, ally))
				{
					engagedHeroes.Add(hero);
					break;
				}
			}
		}

		List<Character> targetPool = engagedHeroes.Count > 0 ? engagedHeroes : allHeroes;
		if (targetPool.Count == 0)
		{
			ShowNoTarget(owningCharacter.token.transform.position);
			Finish();
			return;
		}

		Util.Shuffle(targetPool);
		Character targetHero = targetPool[0];

		owningCharacter.SetFacing(TileGrid.Instance.GetFacingDirection(owningCharacter, targetHero));

		warnedTiles = new List<Tile>(TileGrid.Instance.FindCharacter(targetHero));
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

			AttackCharacters(hitCharacters, new ActionController.AttackProfile(0, 0, 10, true));

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
		
		instructions.Add(new CardInstruction("Move away from near by enemies"));
		instructions.Add(new CardInstruction("Marks a target and <u>Charge</u>"));
		instructions.Add(new CardInstruction());
		instructions.Add(new CardInstruction("On <u>Charge</u> completion: deal 10 damage to characters in marked tiles"));
		DisplayGrid.Instance.Show();
		return instructions;
	}
}
