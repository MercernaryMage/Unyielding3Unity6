using UnityEngine;

public class Doom : StatusEffect
{
    public override void CharacterStartTurn(CharacterStartTurnMessage characterStartTurnMessage)
    {
        if (characterStartTurnMessage.character != character)
        {
            return;
        }
        Destroy(this);
        character.SpendEnergy(4);
    }

    public override string GetDisplayName()
    {
        return "Doom";
    }

    public override string GetEffectText()
    {
        return "At the start of your turn, lose 4 energy.";
    }
}
