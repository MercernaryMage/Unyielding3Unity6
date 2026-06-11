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

		foreach (var turnControlEntry in turnControlEntries)
		{
			Add(turnControlEntry);
		}
	}

	public void Add(TurnControlEntry entry)
	{
		GameObject displayElement = Instantiate(prefab);
		TurnControlDisplayElement element = displayElement.GetComponent<TurnControlDisplayElement>();
		string displayName = entry.character.displayName.Substring(0, 3);
		if (!entry.character.hero && entry.character.displayNumber > 0)
		{
			displayName += entry.character.displayNumber.ToString();
		}
		element.Set(displayName, entry.value);
		
		displayElement.transform.SetParent(content);
		createdObjects.Add(displayElement);
	}
}
