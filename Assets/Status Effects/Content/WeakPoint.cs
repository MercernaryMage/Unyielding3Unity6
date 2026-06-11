using UnityEngine;

public class WeakPoint : StatusEffect
{
	public override string GetDisplayName()
	{
		return "Weak Point";
	}

	public override void OnCharacterAttacking(CharacterAttackingMessage characterAttackingMessage)
	{
		if (characterAttackingMessage.defender == character)
		{
			characterAttackingMessage.autoCrit = true;
			Destroy(this);
		}
	}

	public override string GetEffectText()
	{
		return "The next attack against this target is automatically a critical hit.";
	}
}
