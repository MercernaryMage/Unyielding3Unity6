using UnityEngine;

public class NoEscapeTrait : Trait
{
	public const int triggerCount = 100;

	public override void Start()
	{
		base.Start();
		character.triggerCount = triggerCount;
	}
}
