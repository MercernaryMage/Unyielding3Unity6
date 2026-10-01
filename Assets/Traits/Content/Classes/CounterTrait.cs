using UnityEngine;

public class CounterTrait : Trait
{
	bool usedThisRound;

	public override void CharacterStartTurn(CharacterStartTurnMessage message)
	{
		if (message.character == character)
		{
			usedThisRound = false;
		}
	}

	public override void CharacterAttack(CharacterAttackingMessage message)
	{
		if (usedThisRound)
		{
			return;
		}
		if (message.defender != character || message.attacker == character)
		{
			return;
		}
		if (message.attacker.hero == character.hero)
		{
			return;
		}
		if (!character.alive || character.IsDowned())
		{
			return;
		}

		usedThisRound = true;
		Character attacker = message.attacker;

		ActionController.Instance.queuedActions.Add(() =>
		{
			if (!character.alive || character.IsDowned() || !attacker.alive)
			{
				ActionController.Instance.EndAction();
				return;
			}

			if (scriptableObject != null)
			{
				AICardDisplay.Instance.ShowFakeCard(scriptableObject.displayName, scriptableObject.description);
			}

			BattleController.playerHasControl = false;
			AnimationController.Instance.DelayedCallback(1f, () => DoCounter(attacker));
		});
	}

	void DoCounter(Character attacker)
	{
		if (!character.alive || character.IsDowned() || !attacker.alive)
		{
			ActionController.Instance.EndAction();
			return;
		}

		if (!TileGrid.Instance.CharactersAreAdjacent(character, attacker))
		{
			FloatingCombatNumberController.Instance.QueueFloatingCombatNumber(character, "No target");
			ActionController.Instance.EndAction();
			return;
		}

		character.SetFacing(TileGrid.Instance.GetFacingDirection(character, attacker));
		ActionController.Instance.PlayAttackAnimation(character, null, () =>
		{
			if (character.alive && attacker.alive)
			{
				ActionController.Instance.AttackCharacter(attacker, character, new ActionController.AttackProfile(1, 6, 0));
			}

			ActionController.Instance.EndAction();
		});
	}
}
