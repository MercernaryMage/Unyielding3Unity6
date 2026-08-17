using System;
using System.Collections.Generic;
using UnityEngine;

public class DebugSetDetermination : MonoBehaviour
{
	[Serializable]
	public class DeterminationEntry
	{
		public string characterName;
		public int value;
	}

	public List<DeterminationEntry> healthEntries = new List<DeterminationEntry>();

	public bool runOnce;
	static bool runOnceInternal = true;

	void Start()
	{
		if (!Application.isEditor || !runOnceInternal)
		{
			return;
		}
		if (runOnce)
		{
			runOnceInternal = false;
		}
		Invoke("ApplyDetermination", 1);
	}

	void ApplyDetermination()
	{
		foreach (DeterminationEntry entry in healthEntries)
		{
			if (string.IsNullOrEmpty(entry.characterName))
			{
				continue;
			}
			Character found = FindCharacter(entry.characterName);
			if (found == null)
			{
				continue;
			}
			found.storageCharacter.currentDetermination = entry.value;
		}
	}

	Character FindCharacter(string characterName)
	{
		foreach (Character c in BattleController.Instance.heroes)
		{
			if (c.displayName == characterName)
			{
				return c;
			}
		}
		foreach (Character c in BattleController.Instance.enemies)
		{
			if (c.displayName == characterName)
			{
				return c;
			}
		}
		return null;
	}
}
