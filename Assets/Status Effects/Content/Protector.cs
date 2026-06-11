using System.Collections.Generic;
using UnityEngine;
using static TemplateLibrary;

public class Protector : StatusEffect
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
        if (message.defender == character)
        {
            return;
        }
        if (message.defender.hero != character.hero)
        {
            return;
        }
        if (!TileGrid.Instance.CharactersAreAdjacent(character, message.defender))
        {
            return;
        }
        if (!TileGrid.Instance.CharactersAreAdjacent(character, message.attacker))
        {
            return;
        }
        ActionController.Instance.queuedActions.Add(() => 
        {
            BattleController.playerHasControl = false;
			character.SetFacing(TileGrid.Instance.GetFacingDirection(character, message.attacker));
			ActionController.Instance.PlayAttackAnimation(character, null, () =>
			{
				if (message.defender.alive && character.alive)
				{
                    AICardDisplay.Instance.ShowFakeCard(GetDisplayName(), GetEffectText());
					ActionController.Instance.AttackCharacter(message.attacker, character, new ActionController.AttackProfile(1, 6, 0));
				}

				ActionController.Instance.EndAction();
			});
		}
        );
	}

    public override string GetDisplayName()
    {
        return "Protector";
    }

    public override string GetEffectText()
    {
        return "When an adjacent ally is hit by an adjacent attacker, this character counterattacks.";
    }

	public static List<CardInstruction> GetCardInstructions()
	{
		DisplayGrid.Instance.Clear(11, 8);
		List<CardInstruction> instructions = new List<CardInstruction>();
		instructions.Add(new CardInstruction($"When an adjacent ally is hit by an adjacent attacker, this character counterattacks."));
		instructions.Add(new CardInstruction($"Play this characters next card"));

		return instructions;
	}
}
