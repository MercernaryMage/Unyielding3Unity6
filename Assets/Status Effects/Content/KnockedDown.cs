using UnityEngine;

public class KnockedDown : StatusEffect
{
	public override string GetDisplayName()
	{
		return "Knocked Down";
	}

	public override void Start()
	{
		base.Start();
		character.currentMovement = 0;
		if (MovementController.Instance.running && MovementController.Instance.movingCharacter == character)
		{
			MovementController.Instance.ShowMovement(character);
		}
	}

	public override void CharacterStartTurn(CharacterStartTurnMessage characterStartTurnMessage)
	{
		if (characterStartTurnMessage.character != character)
		{
			return;
		}

		Destroy(this);
		if (character.hero == true)
		{
			character.currentMovement = 0;
		}
	}

	public override void OnCharacterAttacking(CharacterAttackingMessage characterAttackingMessage)
	{
		if (characterAttackingMessage.attacker == character)
		{
			characterAttackingMessage.accuracy -= 1;
			characterAttackingMessage.AddToAccuracyString($"-1 ({GetDisplayName()})");
		}
	}

	public override string GetEffectText()
	{
		return "Loses all movement now and at turn start.  Attacks have -1 accuracy.  Will be removed at the start of the units turn.";
	}
}
