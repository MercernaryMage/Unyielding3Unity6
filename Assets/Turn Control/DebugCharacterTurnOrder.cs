using System;
using System.Collections.Generic;
using UnityEngine;

// Defines an explicit turn order for a battle.
//
// Populate `turnOrder` with character names. Use a character's displayName for a
// specific character (e.g. a hero like "Catelly"). Enemies do not need their
// specific (numbered) name -- instead put the generic token "enemy". Each "enemy"
// entry is matched, in order, against BattleController's enemies list: the first
// "enemy" entry maps to enemies[0], the second to enemies[1], and so on.
//
// Any characters not referenced by the list are appended at the end so nobody is
// dropped from the turn order.
public class DebugCharacterTurnOrder : MonoBehaviour
{
	public const string EnemyToken = "enemy";

	[Tooltip("When true, the battle uses this explicit order. When false, the game falls back to rolling GetInitiative() for each character.")]
	public bool useExplicitOrder = true;

	[Tooltip("Ordered character names. Use a character's name, or \"enemy\" to consume the next enemy from BattleController's enemies list.")]
	public List<string> turnOrder = new List<string>();

	public List<Character> GetOrderedCharacters()
	{
		List<Character> heroes = BattleController.Instance.heroes;
		List<Character> enemies = BattleController.Instance.enemies;

		List<Character> result = new List<Character>();
		int enemyIndex = 0;

		foreach (string entry in turnOrder)
		{
			if (string.Equals(entry, EnemyToken, StringComparison.OrdinalIgnoreCase))
			{
				if (enemyIndex < enemies.Count)
				{
					result.Add(enemies[enemyIndex]);
					enemyIndex++;
				}
				else
				{
					Debug.LogWarning($"DebugCharacterTurnOrder: more \"{EnemyToken}\" entries than enemies; extra entry ignored.");
				}
				continue;
			}

			Character match = FindByName(heroes, entry);
			if (match == null)
			{
				match = FindByName(enemies, entry);
			}

			if (match != null && !result.Contains(match))
			{
				result.Add(match);
			}
			else if (match == null)
			{
				Debug.LogWarning($"DebugCharacterTurnOrder: no character found matching name \"{entry}\".");
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
}
