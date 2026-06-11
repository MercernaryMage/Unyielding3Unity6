using UnityEngine;

public class RangeResistanceTrait : Trait
{
    public override void PreDamageDealt(PreDamageDealtMessage message)
    {
        if (message.defender != character)
        {
            return;
        }
        if (!message.ranged)
        {
            return;
        }
        message.hasResistance = true;
    }
}
