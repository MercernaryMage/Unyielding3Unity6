using UnityEngine;

public class Injured : StatusEffect
{
	public int magnitude;

	public override void EffectBeingRemoved()
	{
		character.toughness += magnitude;
	}

	public override void DoStack(StatusEffectInitData data)
	{
		if (data == null)
		{
			return;
		}
		character.toughness -= data.magnitude;
		magnitude += data.magnitude;
	}

	public override string GetExplanationName()
	{
		return "Injured";
	}

}
