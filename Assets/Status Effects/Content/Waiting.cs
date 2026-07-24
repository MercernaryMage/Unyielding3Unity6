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
                    AICardDisplay.Instance.ShowFakeCard(GetExplanationName(), GetExplanation().explanationContent);
                    ActionController.AttackProfile profile = new ActionController.AttackProfile(1, 6, 0);
                    profile.trigger = true;
                    ActionController.Instance.AttackCharacter(attacker, character, profile);
                }

                ActionController.Instance.EndAction();
            });
        });
    }

    public override string GetExplanationName()
    {
        return "Waiting";
    }

}
