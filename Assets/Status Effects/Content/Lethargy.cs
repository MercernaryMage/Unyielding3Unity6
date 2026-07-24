using UnityEngine;

public class Lethargy : StatusEffect
{
    int lastMovement;

	public override void CharacterStartTurn(CharacterStartTurnMessage characterStartTurnMessage)
    {
        if (characterStartTurnMessage.character != character)
        {
            return;
        }
        lastMovement = character.currentMovement;
    }

    public override void OnCharacterFinishedMoving(CharacterFinishedMovingMessage characterFinishedMovingMessage)
    {
        if (characterFinishedMovingMessage.movingCharacter != character)
        {
            return;
        }
        int tilesMoved = lastMovement - character.currentMovement;
        lastMovement = character.currentMovement;
        if (tilesMoved <= 0)
        {
            return;
        }
        ActionController.AttackResults results = new ActionController.AttackResults();
        ActionController.AttackProfile profile = new ActionController.AttackProfile(0, 0, 0);
        profile.guaranteed = tilesMoved;
        ActionController.Instance.DamageCharacter(character, character, profile, results);
        CombatLogControl.Instance.AddEntry($"Lethargy deals {results.damageDealt} to {character.displayName}");
    }

    public override void OnPreviewMovementDamage(PreviewMovementDamageMessage previewMovementDamageMessage)
    {
        if (previewMovementDamageMessage.movingCharacter != character)
        {
            return;
        }
        previewMovementDamageMessage.damage += previewMovementDamageMessage.tilesMoved;
    }

    public override void CharacterEndTurn(CharacterEndTurnMessage characterEndTurnMessage)
    {
        if (characterEndTurnMessage.character != character)
        {
            return;
        }
        Destroy(this);
    }

    public override string GetExplanationName()
    {
        return "Lethargy";
    }

}
