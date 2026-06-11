using System.Collections.Generic;
using UnityEngine;

public class ArmorUp : Card
{
    public override void Execute()
    {
        int amount = GetIntValue("Amount");
        if (owningCharacter.armor < amount)
        {
            owningCharacter.armor = amount;
        }
        Finish();
    }

    public static List<CardInstruction> GetCardInstructions(CardScriptableObject scriptableObject)
    {
        int amount = scriptableObject.GetTagIntValue("Amount");
        DisplayGrid.Instance.Clear(11, 8);
        List<CardInstruction> instructions = new List<CardInstruction>();
        instructions.Add(new CardInstruction($"If armor is below {amount}, set armor to {amount}"));
        DisplayGrid.Instance.Show();
        return instructions;
    }
}
