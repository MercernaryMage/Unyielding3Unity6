using System.Reflection;
using UnityEngine;

public class DebugSetStat : MonoBehaviour
{
	public string characterName;
	public string statName;
	public int value;

	private void Start()
	{
		Invoke("Setup", .5f);
	}

	public void Setup()
	{
		Character target = null;
		foreach (Character c in BattleController.Instance.heroes)
		{
			if (c.displayName == characterName)
			{
				target = c;
				break;
			}
		}

		if (target == null)
		{
			Debug.Log($"DebugSetStat: failed to find hero named {characterName}");
			return;
		}

		FieldInfo field = typeof(Character).GetField(statName);
		if (field == null)
		{
			Debug.Log($"DebugSetStat: failed to find field named {statName}");
			return;
		}

		field.SetValue(target, value);
	}
}
