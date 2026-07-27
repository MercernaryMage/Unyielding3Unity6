using UnityEngine;

public class Targeted : StatusEffect
{
    public Character causingCharacter;

    public override void CharacterStartTurn(CharacterStartTurnMessage message)
    {
        if (!character.alive)
		{
            Destroy(this);
            return;
		}
		if (message.character == character)
		{
			MovementController.Instance.ShowCharacterWarning(character, "!!!!!");
			return;
		}
		if (message.character != causingCharacter)
        {
            return;
        }

        message.turnStartLocks.Add(this);
        ActionController.Instance.PlayAttackAnimation(causingCharacter, null, () =>
        {
            TurnControl.Instance.RemoveLock(this);
			ActionController.Instance.AttackCharacter(character, causingCharacter,
			new ActionController.AttackProfile(0, 0, 10));
		});
    }

    public override void OnCharacterFinishedMoving(CharacterFinishedMovingMessage message)
    {
        if (message.movingCharacter == character)
        {
            Destroy(this);
        }
    }

    public override void CharacterEndTurn(CharacterEndTurnMessage message)
    {
        if (message.character == character)
        {
            MovementController.Instance.HideCharacterWarning();
        }
    }

    public override void EffectBeingRemoved()
    {
        if (MovementController.Instance != null)
        {
            MovementController.Instance.HideCharacterWarning();
        }
    }

    public override string GetExplanationName()
    {
        return "Targeted";
    }

}
