using UnityEngine;

public class Targeted : StatusEffect
{
    public Character causingCharacter;

    public override void CharacterStartTurn(CharacterStartTurnMessage message)
    {
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

    public override string GetExplanationName()
    {
        return "Targeted";
    }

}
