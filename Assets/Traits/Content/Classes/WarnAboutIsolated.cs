using UnityEngine;

public class WarnAboutIsolated : Trait
{
	public override void Start()
	{
		base.Start();
		foreach (Character c in BattleController.Instance.heroes)
		{
			if (Stalk.IsIsolated(c))
			{
				c.AddStatusEffect(typeof(Isolated), null);
			}
		}
	}

	public override void OnCharacterFinishedMoving(CharacterFinishedMovingMessage characterFinishedMovingMessage)
	{
		if (Stalk.IsIsolated(characterFinishedMovingMessage.movingCharacter))
		{
			characterFinishedMovingMessage.movingCharacter.AddStatusEffect(typeof(Isolated), null);
		}
	}
}
