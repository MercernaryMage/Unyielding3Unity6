using UnityEngine;

public class LockedOn : StatusEffect
{
    public Character causingCharacter;
	bool useable = true;

	public override void CharacterStartTurn(CharacterStartTurnMessage message)
	{
		if (!causingCharacter.alive)
		{
			Destroy(this);
			return;
		}
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
			if (!character.alive || !causingCharacter.alive)
			{
				ActionController.Instance.EndAction();
				return;
			}
			BattleController.playerHasControl = false;
			causingCharacter.SetFacing(TileGrid.Instance.GetFacingDirection(causingCharacter, character));
			ActionController.Instance.PlayAttackAnimation(causingCharacter, null, () =>
			{
				if (character.alive && causingCharacter.alive)
				{
					AICardDisplay.Instance.ShowFakeCard(GetDisplayName(), GetEffectText());
					ActionController.Instance.AttackCharacter(character, causingCharacter, new ActionController.AttackProfile(1, 6, 0));
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
        return "Once per turn, when this character finishes moving or attacking, the causing character will attack them.";
    }
}
