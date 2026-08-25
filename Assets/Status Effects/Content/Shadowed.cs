using UnityEngine;

public class Shadowed : StatusEffect
{
	public Character causingCharacter;
	bool useable = true;

	public override void CharacterStartTurn(CharacterStartTurnMessage message)
	{
		if (causingCharacter == null || !causingCharacter.alive)
		{
			Destroy(this);
			return;
		}
		useable = true;
	}

	public override void OnCharacterFinishedMoving(CharacterFinishedMovingMessage message)
	{
		if (message.movingCharacter != character)
		{
			return;
		}
		if (causingCharacter == null || !causingCharacter.alive)
		{
			Destroy(this);
			return;
		}
		if (TileGrid.Instance.DoesCharacterHaveLOSToCharacter(character, causingCharacter))
		{
			return;
		}
		if (!useable)
		{
			return;
		}
		useable = false;
		ActionController.Instance.AttackCharacter(character, causingCharacter,
			new ActionController.AttackProfile(1, 6, 0, true));
	}

	public override void CharacterEndTurn(CharacterEndTurnMessage message)
	{
		if (message.character != character)
		{
			return;
		}
		if (causingCharacter == null || !causingCharacter.alive)
		{
			Destroy(this);
			return;
		}
		if (TileGrid.Instance.DoesCharacterHaveLOSToCharacter(character, causingCharacter))
		{
			if (character.GetComponent<LockedOn>() != null)
			{
				ActionController.AttackResults results = new ActionController.AttackResults();
				ActionController.AttackProfile profile = new ActionController.AttackProfile(0, 0, 0);
				profile.guaranteed = 4;
				ActionController.Instance.DamageCharacter(character, causingCharacter, profile, results);
				CombatLogControl.Instance.AddEntry($"Shadowed deals {results.damageDealt} to {character.displayName}");
			}
			LockedOn lockedOn = (LockedOn)character.AddStatusEffect(typeof(LockedOn), null);
			lockedOn.causingCharacter = causingCharacter;
			Destroy(this);
		}
	}

	public override string GetExplanationName()
	{
		return "Shadowed";
	}

}
