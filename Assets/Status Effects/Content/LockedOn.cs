using UnityEngine;

public class LockedOn : StatusEffect
{
    public Character causingCharacter;
	bool useable = true;

	public override void CharacterStartTurn(CharacterStartTurnMessage message)
	{
		useable = true;
	}

	public override void OnCharacterFinishedMoving(CharacterFinishedMovingMessage message)
    {
		if (message.movingCharacter == character)
		{
			
			if (TileGrid.Instance.DoesCharacterHaveLOSToCharacter(character, causingCharacter))
			{
				if (!useable)
				{
					return;
				}
				useable = false;
				ActionController.Instance.AttackCharacter(character, causingCharacter,
			new ActionController.AttackProfile(1, 6, 0));
			}
			else
			{
				Destroy(this);
			}
		}
	}

	public override void OnAttackComplete(AttackCompleteMessage message)
	{
		if (message.attacker != character)
		{
			return;
		}
		if (!useable)
		{
			return;
		}
		useable = false;
		ActionController.Instance.queuedActions.Add(() =>
		{
			BattleController.playerHasControl = false;
			character.SetFacing(TileGrid.Instance.GetFacingDirection(character, message.attacker));
			ActionController.Instance.PlayAttackAnimation(character, null, () =>
			{
				if (message.defender.alive && character.alive)
				{
					AICardDisplay.Instance.ShowFakeCard(GetDisplayName(), GetEffectText());
					ActionController.Instance.AttackCharacter(message.attacker, character, new ActionController.AttackProfile(1, 6, 0));
				}

				ActionController.Instance.EndAction();
			});
		}
		);
	}

	public override string GetDisplayName()
    {
        return "Locked On";
    }

    public override string GetEffectText()
    {
        return "sadfsadfsad";
    }
}
