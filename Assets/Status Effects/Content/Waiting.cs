using UnityEngine;

public class Waiting : StatusEffect
{
    public override void CharacterStartTurn(CharacterStartTurnMessage characterStartTurnMessage)
    {
        if (characterStartTurnMessage.character == character)
        {
            Destroy(this);
        }
    }

    public override void OnAttackComplete(AttackCompleteMessage message)
    {
        if (message.defender != character)
        {
            return;
        }

        if (!character.alive || character.IsDowned())
        {
            return;
        }

        Character attacker = message.attacker;

        ActionController.Instance.queuedActions.Add(() =>
        {
            BattleController.playerHasControl = false;
            character.SetFacing(TileGrid.Instance.GetFacingDirection(character, attacker));
            ActionController.Instance.PlayAttackAnimation(character, null, () =>
            {
                if (character.alive && attacker.alive)
                {
                    AICardDisplay.Instance.ShowFakeCard(GetDisplayName(), GetEffectText());
                    ActionController.AttackProfile profile = new ActionController.AttackProfile(1, 6, 0);
                    profile.trigger = true;
                    ActionController.Instance.AttackCharacter(attacker, character, profile);
                }

                ActionController.Instance.EndAction();
            });
        });
    }

    public override string GetDisplayName()
    {
        return "Waiting";
    }

    public override string GetEffectText()
    {
        return "When attacked, counter-attack the attacker. Removed at the start of this character's turn.";
    }
}
