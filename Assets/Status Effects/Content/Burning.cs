using UnityEngine;

public class Burning : StatusEffect
{
	public int magnitude;

	public override void CharacterEndTurn(CharacterEndTurnMessage characterEndTurnMessage)
	{
		if (characterEndTurnMessage.character != character)
		{
			return;
		}

		CharacterTakingBurnDamageMessage burnMessage = new CharacterTakingBurnDamageMessage();
		burnMessage.character = character;
		MessagePump.Instance.SendMessage(burnMessage);

		int roll = Util.RollDice(1, 20);
		if (roll > 10 && !burnMessage.autoFail)
		{
			FloatingCombatNumberController.Instance.QueueFloatingCombatNumber(character, "Burning removed");
			Destroy(this);
			return;
		}

		ActionController.AttackResults results = new ActionController.AttackResults();
		ActionController.AttackProfile profile = new ActionController.AttackProfile(0, 0, magnitude);
		profile.breach = true;
		ActionController.Instance.DamageCharacter(character, character, profile, results);
	}

	public override void DoStack(StatusEffectInitData data)
	{
		magnitude += data.magnitude;
	}

	public override string GetDisplayName()
	{
		return "Burning";
	}

	public override string GetEffectText()
	{
		return $"At end of turn, roll d20. On 10 or lower, take {magnitude} damage.";
	}
}
