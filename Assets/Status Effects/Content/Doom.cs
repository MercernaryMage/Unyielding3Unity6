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

    public override string GetExplanationName()
    {
        return "Doom";
    }

}
