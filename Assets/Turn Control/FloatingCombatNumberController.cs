using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using UnityEngine;

public class FloatingCombatNumberController : SceneSingleton<FloatingCombatNumberController>
{
	class FloatingCombatData
	{
		public Vector3 tokenPosition;
		public float time;
		public List<string> messages = new List<string>();
	}

	public GameObject FloatingCombatNumberPrefab;

	Dictionary<Character, FloatingCombatData> data = new Dictionary<Character, FloatingCombatData>();

	float messageDelay = .8f;

	void Update()
	{
		foreach (KeyValuePair<Character, FloatingCombatData> combatData in data)
		{
			combatData.Value.time -= Time.deltaTime;
			if (combatData.Value.time <= 0 && combatData.Value.messages.Count > 0)
			{
				
				ShowFloatingCombatNumber(combatData.Value.tokenPosition +
					Vector3.up * 1.5f * combatData.Key.characterDefinition.size, combatData.Value.messages[0]);
				combatData.Value.messages.RemoveAt(0);
				combatData.Value.time = messageDelay;
			}
		}
	}

	public void QueueFloatingCombatNumber(Character c, string message)
	{
		if (c.token == null)
		{
			return;
		}
		if (data.ContainsKey(c))
		{
			data[c].messages.Add(message);
			data[c].tokenPosition = c.token.transform.position;
		}
		else
		{
			FloatingCombatData floatingCombatData = new FloatingCombatData();
			floatingCombatData.messages.Add(message);
			floatingCombatData.tokenPosition = c.token.transform.position;
			data[c] = (floatingCombatData);
		}
	}

	void ShowFloatingCombatNumber(Vector3 position, string number)
	{
		GameObject obj = Instantiate(FloatingCombatNumberPrefab);
		obj.GetComponent<FloatingCombatNumber>().Set(position, number);
	}
}
