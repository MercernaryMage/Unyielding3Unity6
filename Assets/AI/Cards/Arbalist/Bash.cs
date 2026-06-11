using System;
using System.Collections.Generic;
using UnityEngine;

public class Bash : Card
{
    TemplateLibrary.TilesAndDirection tilesAndDirection;

    public override void Execute()
    {
        List<Tile> adjacentTiles = TemplateLibrary.GetAdjacentCharacterTarget(owningCharacter, null);
        if (adjacentTiles == null)
        {
            Util.BringCardToTopOfDeck(owningCharacter, "Snipe");
            AIController.Instance.TakeTurn(owningCharacter);
            return;
        }

        tilesAndDirection = new TemplateLibrary.TilesAndDirection(adjacentTiles, Direction.East);

        AnimationController.Instance.ShowTiles(tilesAndDirection.tiles, Tile.OverlayType.PossibleAttck, ReturnFromShowingAttackTiles);
    }

    void ReturnFromShowingAttackTiles()
    {
        owningCharacter.SetFacing(TileGrid.Instance.GetFacingDirection(owningCharacter, tilesAndDirection.tiles[0].character));
        ActionController.Instance.PlayAttackAnimation(owningCharacter, null, () =>
        {
            foreach (Tile t in tilesAndDirection.tiles)
            {
                t.HideOverlay(Tile.OverlayType.PossibleAttck);
                if (t.character != null && t.character.hero)
                {
                    ActionController.AttackResults results = ActionController.Instance.AttackCharacter(
                        t.character, owningCharacter, new ActionController.AttackProfile(1, 6, 0));
                    if (results.hit)
                    {
						ActionController.Instance.KnockBack(t.character, TileGrid.Instance.FindCharacter(owningCharacter), 1);
                    }
                }
            }
            Util.BringCardToTopOfDeck(owningCharacter, "Snipe");
            AIController.Instance.TakeTurn(owningCharacter);
        });
    }

    public static List<CardInstruction> GetCardInstructions(CardScriptableObject scriptableObject)
    {
        DisplayGrid.Instance.Clear(11, 8);
        List<CardInstruction> instructions = new List<CardInstruction>();
        instructions.Add(new CardInstruction("Attack an adjacent enemy for 1d6 damage"));
        instructions.Add(new CardInstruction("On hit, knockback 1"));
        DisplayGrid.Instance.Show();
        return instructions;
    }
}
