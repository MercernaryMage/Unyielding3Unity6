using System.Collections.Generic;
using UnityEngine;

public class ArmorUp : Card
{
    public override void Execute()
    {
        AnimationController.Instance.ShowTiles(null, Tile.OverlayType.PossibleMovement, Delay, null, 3);
    }

    void Delay()
    {
        int amount = GetIntValue("Amount");
        if (owningCharacter.armor < amount)
        {
            owningCharacter.armor = amount;
        }
        AIController.Instance.Reshuffle(owningCharacter);
        AIController.Instance.TakeTurn(owningCharacter);
    }

    public static List<CardInstruction> GetCardInstructions(CardScriptableObject scriptableObject)
    {
        int amount = scriptableObject.GetTagIntValue("Amount");
        DisplayGrid.Instance.Clear(11, 8);
        List<CardInstruction> instructions = new List<CardInstruction>();
        instructions.Add(new CardInstruction($"If armor is below {amount}, set armor to {amount}"));
        instructions.Add(new CardInstruction("Do next action"));
        DisplayGrid.Instance.Show();
        return instructions;
    }
}
