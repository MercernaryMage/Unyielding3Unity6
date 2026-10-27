using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnControlDisplay : SceneSingleton<TurnControlDisplay>
{
	public Transform content;
	public GameObject prefab;

	List<System.Tuple<Character, GameObject>> createdObjects = new List<System.Tuple<Character, GameObject>>();

	public void Set(List<TurnControlEntry> turnControlEntries)
	{
		foreach (System.Tuple<Character, GameObject> obj in createdObjects)
		{
			Destroy(obj.Item2);
		}
		createdObjects.Clear();

		for (int i = 0; i < turnControlEntries.Count; i++)
		{
			Add(turnControlEntries[i], i == 0, i == turnControlEntries.Count - 1);
		}
	}

	public void Add(TurnControlEntry entry, bool first, bool last)
	{
		GameObject displayElement = Instantiate(prefab);
		TurnControlDisplayElement element = displayElement.GetComponent<TurnControlDisplayElement>();
		element.Set(entry.character, TurnControl.Instance.GetDisplayValue(entry.value), first, last, true);

		displayElement.transform.SetParent(content);
		displayElement.transform.localScale = Vector3.one;
		createdObjects.Add(new System.Tuple<Character, GameObject>(entry.character, displayElement));
	}

	public void ChangeHighlightState(Character c, bool state)
	{
		foreach (System.Tuple<Character, GameObject> tuple in createdObjects)
		{
			if (tuple.Item1 == c)
			{
				tuple.Item2.GetComponent<TurnControlDisplayElement>().highlight.SetActive(state);
			}
		}
	}
}
//2560 * x = 1920
//x = 1920 / 2560
