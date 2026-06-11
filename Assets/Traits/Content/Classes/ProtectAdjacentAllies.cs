public class ProtectAdjacentAllies : Trait
{
	public override void CharacterAttack(CharacterAttackingMessage message)
	{
		if (message.defender != character && message.defender.hero == character.hero)
		{
			if (TileGrid.AreCharactersAdjacent(message.defender, character))
			{
				message.accuracy -= 2;
				message.AddToAccuracyString($"-2 ({scriptableObject.displayName})");
			}
		}
	}
}
