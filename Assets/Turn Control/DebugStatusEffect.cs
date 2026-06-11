using UnityEngine;
using static StatusEffect;

public class DebugStatusEffect : MonoBehaviour
{
	public string statusEffectName;
	public string characterName;

	void Start()
	{
		if (!Application.isEditor)
		{
			return;
		}
		Invoke("ApplyStatusEffect", 1);
	}

	void ApplyStatusEffect()
	{
		if (string.IsNullOrEmpty(statusEffectName) || string.IsNullOrEmpty(characterName))
		{
			return;
		}
		Character found = null;
		foreach (Character c in BattleController.Instance.heroes)
		{
			if (c.displayName == characterName)
			{
				found = c;
				break;
			}
		}
		if (found == null)
		{
			foreach (Character c in BattleController.Instance.enemies)
			{
				if (c.displayName == characterName)
				{
					found = c;
					break;
				}
			}
		}
		if (found == null)
		{
			return;
		}
		found.AddStatusEffect(System.Type.GetType(statusEffectName), new StatusEffectInitData());
	}
}
