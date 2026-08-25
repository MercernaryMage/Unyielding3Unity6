public class InterventionTrait : Trait
{
	public override void CharacterAttack(CharacterAttackingMessage message)
	{
		if (message.attacker == character)
		{
			return;
		}

		if (!TileGrid.GetCharactersOnTiles(TileGrid.Instance.GetSurroundingTiles(character)).Contains(message.attacker))
		{
			return;
		}

		if (message.attacker.hero == character.hero)
		{
			message.accuracy += 1;
			message.AddToAccuracyString($"+1 ({scriptableObject.displayName})");
		}
		else
		{
			message.accuracy -= 1;
			message.AddToAccuracyString($"-1 ({scriptableObject.displayName})");
		}
	}
}
