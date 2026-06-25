using UnityEngine;
using UnityEngine.Windows.WebCam;

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
		string rollText = $"{character.displayName} rolls {roll} to remove Burning (10 or lower fails)";
		if (burnMessage.autoPass)
		{
			rollText += " (auto pass)";
		}
		if (burnMessage.autoFail)
		{
			rollText += " (auto fail)";
		}
		CombatLogControl.Instance.AddEntry(rollText);

		if ((roll > 10 || burnMessage.autoPass) && !burnMessage.autoFail)
		{
			CombatLogControl.Instance.AddEntry($"Burning removed from {character.displayName}");
			Destroy(this);
			return;
		}

		ActionController.AttackResults results = new ActionController.AttackResults();
		ActionController.AttackProfile profile = new ActionController.AttackProfile(0, 0, magnitude);
		profile.breach = true;
		results.fakeHit = true;
		ActionController.Instance.DamageCharacter(character, character, profile, results);
		CombatLogControl.Instance.AddEntry($"Burning deals {results.damageDealt} to {character.displayName}");
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
