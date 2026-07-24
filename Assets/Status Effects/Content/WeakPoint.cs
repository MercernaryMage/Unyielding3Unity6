using UnityEngine;

public class WeakPoint : StatusEffect
{
	public override string GetExplanationName()
	{
		return "Weak Point";
	}

	public override void OnCharacterAttacking(CharacterAttackingMessage characterAttackingMessage)
	{
		if (characterAttackingMessage.defender == character)
		{
			characterAttackingMessage.autoCrit = true;
		}
	}

	public override void OnAttackComplete(AttackCompleteMessage attackCompleteMessage)
	{
		if (attackCompleteMessage.defender == character &&
			attackCompleteMessage.hit)
		{
			Destroy(this);
		}
	}

}
