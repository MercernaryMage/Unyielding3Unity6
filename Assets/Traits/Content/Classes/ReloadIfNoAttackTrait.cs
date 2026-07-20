public class ReloadIfNoAttackTrait : Trait
{
	bool hasAttacked = false;

	public override void CharacterStartTurn(CharacterStartTurnMessage message)
	{
		if (message.character != character)
		{
			return;
		}
		hasAttacked = false;
	}

	public override void AttackComplete(AttackCompleteMessage message)
	{
		if (message.attacker == character)
		{
			hasAttacked = true;
		}
	}

	public override void CharacterEndTurn(CharacterEndTurnMessage message)
	{
		if (message.character != character)
		{
			return;
		}
		if (hasAttacked || character.storageCharacter == null)
		{
			return;
		}

		FloatingCombatNumberController.Instance.QueueFloatingCombatNumber(character, "reload");

		foreach (Item item in character.storageCharacter.equipment)
		{
			if (item.itemDefinition == null)
			{
				continue;
			}
			if (!item.loaded)
			{
				item.Load();
			}
		}
	}
}
