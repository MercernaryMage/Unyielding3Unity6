using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnControlDisplay : SceneSingleton<TurnControlDisplay>
{
	public Transform content;
	public GameObject prefab;

	List<GameObject> createdObjects = new List<GameObject>();

	public void Set(List<TurnControlEntry> turnControlEntries)
	{
		foreach (GameObject obj in createdObjects)
		{
			Destroy(obj);
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
		element.Set(entry.character.hero, entry.value, entry.character.characterDefinition.battlePortrait, first, last, true);

		displayElement.transform.SetParent(content);
		displayElement.transform.localScale = Vector3.one;
		createdObjects.Add(displayElement);
	}
}
//2560 * x = 1920
//x = 1920 / 2560
