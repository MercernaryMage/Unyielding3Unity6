using System;
using System.Collections.Generic;
using UnityEngine;

// Defines an explicit turn order for a battle.
//
// Populate `turnOrder` with character names. Use a character's displayName for a
// specific character (e.g. a hero like "Catelly"). Enemies can be named either by
// their numbered displayName ("Bandit 2") or by their base name ("Bandit"), which
// takes the next enemy of that type that has not been placed yet. The generic token
// "enemy" takes the next enemy of any type that has not been placed yet.
//
// Any characters not referenced by the list are appended at the end so nobody is
// dropped from the turn order.
public class DebugCharacterTurnOrder : MonoBehaviour
{
	public const string EnemyToken = "enemy";

	[Tooltip("Ordered character names. Use a character's name, an enemy's base name to take the next enemy of that type, or \"enemy\" to take the next enemy of any type.")]
	public List<string> turnOrder = new List<string>();

	public List<Character> GetOrderedCharacters()
	{
		List<Character> heroes = BattleController.Instance.heroes;
		List<Character> enemies = BattleController.Instance.enemies;

		List<Character> claimed = new List<Character>();
		List<Character> slots = new List<Character>();

		foreach (string entry in turnOrder)
		{
			if (string.Equals(entry, EnemyToken, StringComparison.OrdinalIgnoreCase))
			{
				slots.Add(null);
				continue;
			}

			Character match = FindByName(heroes, entry);
			if (match == null)
			{
				match = FindEnemyByName(enemies, claimed, entry);
			}

			if (match == null)
			{
				Debug.LogWarning($"DebugCharacterTurnOrder: no character found matching name \"{entry}\".");
				continue;
			}

			if (claimed.Contains(match))
			{
				continue;
			}

			claimed.Add(match);
			slots.Add(match);
		}

		List<Character> result = new List<Character>();
		foreach (Character slot in slots)
		{
			if (slot != null)
			{
				result.Add(slot);
				continue;
			}

			Character nextEnemy = FindNextUnusedEnemy(enemies, claimed);
			if (nextEnemy != null)
			{
				claimed.Add(nextEnemy);
				result.Add(nextEnemy);
			}
		}

		// Make sure every character still gets a turn even if the list missed them.
		AppendMissing(result, heroes);
		AppendMissing(result, enemies);

		return result;
	}

	void AppendMissing(List<Character> result, List<Character> characters)
	{
		foreach (Character c in characters)
		{
			if (!result.Contains(c))
			{
				result.Add(c);
			}
		}
	}

	Character FindByName(List<Character> characters, string name)
	{
		foreach (Character c in characters)
		{
			if (string.Equals(c.displayName, name, StringComparison.OrdinalIgnoreCase))
			{
				return c;
			}
		}
		return null;
	}

	Character FindEnemyByName(List<Character> enemies, List<Character> claimed, string name)
	{
		foreach (Character c in enemies)
		{
			if (claimed.Contains(c))
			{
				continue;
			}
			if (string.Equals(c.displayName, name, StringComparison.OrdinalIgnoreCase))
			{
				return c;
			}
		}
		foreach (Character c in enemies)
		{
			if (claimed.Contains(c))
			{
				continue;
			}
			if (string.Equals(c.characterDefinition.displayName, name, StringComparison.OrdinalIgnoreCase))
			{
				return c;
			}
		}
		return FindByName(enemies, name);
	}

	Character FindNextUnusedEnemy(List<Character> enemies, List<Character> claimed)
	{
		foreach (Character c in enemies)
		{
			if (!claimed.Contains(c))
			{
				return c;
			}
		}
		return null;
	}
}
