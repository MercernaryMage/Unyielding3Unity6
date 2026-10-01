using UnityEngine;

public class BranchingTrait : Trait
{
	public override void StatusEffectRemoved(StatusEffectRemovedMessage message)
	{
		if (message.effect.GetType() != typeof(Paralyzed) && message.effect.GetType() != typeof(Staked))
		{
			return;
		}

		Character damagedCharacter = message.character;
		if (damagedCharacter == null || !damagedCharacter.alive || damagedCharacter.IsDowned())
		{
			return;
		}

		ActionController.AttackProfile profile = new ActionController.AttackProfile(0, 0, 1);
		profile.ignoreToughness = true;

		ActionController.AttackResults results = new ActionController.AttackResults();
		results.fakeHit = true;
		ActionController.Instance.DamageCharacter(damagedCharacter, character, profile, results);
	}
}
