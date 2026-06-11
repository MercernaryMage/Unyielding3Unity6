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

    public override string GetDisplayName()
    {
        return "Targeted";
    }

    public override string GetEffectText()
    {
        return "At the start of the causing character's turn, take 10 damage on hit. Removed when this character moves.";
    }
}
