using UnityEngine;

public class Isolated : StatusEffect
{
	public override void Start()
	{
		base.Start();
		GameObject obj = Instantiate(BattleController.Instance.isolatedEffect);
		obj.transform.SetParent(character.token.transform);
		obj.transform.localPosition = new Vector3(0, 1.5f, -1);
	}

	public override void OnCharacterFinishedMoving(CharacterFinishedMovingMessage characterFinishedMovingMessage)
	{
		if (!Stalk.IsIsolated(character))
		{
			Destroy(this);
		}
	}

	public override string GetStatusEffectKey()
	{
		return "Isolated";
	}
}
