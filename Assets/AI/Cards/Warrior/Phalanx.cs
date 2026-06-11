using System.Collections.Generic;
using UnityEngine;

public class Phalanx : Card
{
    public override void Execute()
    {
        GiveArmor(owningCharacter);

        foreach (Tile t in TileGrid.Instance.GetAllAdjacentTilesToCharacter(owningCharacter))
        {
            if (t.character == null || !t.character.alive || t.character.IsDowned())
            {
                continue;
            }

            if (BattleController.Instance.enemies.Contains(t.character))
            {
                GiveArmor(t.character);
            }
        }

        AnimationController.Instance.ShowTiles(null, Tile.OverlayType.PossibleMovement, Finish);
    }

    void GiveArmor(Character c)
    {
        if (c.armor < 3)
        {
            c.armor = 3;
        }
    }

    public static List<CardInstruction> GetCardInstructions(CardScriptableObject scriptableObject)
    {
        DisplayGrid.Instance.Clear(11, 8);
        List<CardInstruction> instructions = new List<CardInstruction>();
        instructions.Add(new CardInstruction("Give self and adjacent allies 3 armor"));
        instructions.Add(new CardInstruction("Will not exceed 3 armor"));
        DisplayGrid.Instance.Show();
        return instructions;
    }
}
