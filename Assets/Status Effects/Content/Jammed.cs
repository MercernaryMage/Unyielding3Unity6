using UnityEngine;

public class Jammed : StatusEffect
{
    public override string GetDisplayName()
    {
        return "Jammed";
    }

    public override string GetEffectText()
    {
        return "Cannot make ranged attacks.";
    }
}
