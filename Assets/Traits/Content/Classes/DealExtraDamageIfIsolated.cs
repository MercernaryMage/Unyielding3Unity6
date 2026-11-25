public class DealExtraDamageIfIsolated : Trait
{
	public override void PreDamageDealt(PreDamageDealtMessage message)
	{
		if (message.attacker == character)
		{
			if (TileGrid.Instance.IsTargetIsolated(message.defender))
			{
				message.damage += 5;
			}
		}
	}
}
