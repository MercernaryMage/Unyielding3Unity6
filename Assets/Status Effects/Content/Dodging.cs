using UnityEngine;

public class Dodging : StatusEffect
{
	public override void Start()
	{
		base.Start();
		character.currentEvasion += 2;
	}

	public override void OnCharacterMiss(CharacterMissMessage characterMissMessage)
	{
		if (characterMissMessage.defender == character)
		{
			character.currentEvasion -= 2;
			Destroy(this);
		}
	}

	public override string GetExplanationName()
	{
		return "Dodging";
	}

}
