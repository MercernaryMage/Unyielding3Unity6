using UnityEngine;

public class AttackAgainTrait : Trait
{
    int damageForThisCard = 0;
    bool usedThisCard = false;

    public override void CharacterStartTurn(CharacterStartTurnMessage message)
    {
        if (message.character == character)
        {
            usedThisCard = false;
        }
    }

    public override void CardStart(CardStartMessage message)
    {
		damageForThisCard = 0;
    }

    public override void DamageDealt(DamageDealtMessage message)
    {
        if (usedThisCard)
		{
            return;
		}
		if (message.attacker != character)
        {
            return;
        }
        damageForThisCard += message.damage;
        if (damageForThisCard < 7)
        {
            return;
        }
		usedThisCard = true;
		ActionController.Instance.queuedActions.Add(() =>
        {    
            AIController.Instance.Reshuffle(character);
            AIController.Instance.TakeTurn(character);
        });
    }
}
