using System;
using System.Collections.Generic;
using UnityEngine;

public class DebugSetHealth : MonoBehaviour
{
	[Serializable]
	public class HealthEntry
	{
		public string characterName;
		public int health;
	}

	public List<HealthEntry> healthEntries = new List<HealthEntry>();

	void Start()
	{
		if (!Application.isEditor)
		{
			return;
		}
		Invoke("ApplyHealth", 1);
	}

	void ApplyHealth()
	{
		foreach (HealthEntry entry in healthEntries)
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
			found.currentHP = entry.health;
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
