using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StatusEffectDisplayGroup : MonoBehaviour
{
    public GameObject displayItemPrefab;

	public Transform content;

	List<GameObject> createdObjects = new List<GameObject>();
	List<GameObject> objectsToDestroy = new List<GameObject>();
	bool awaitingDestroy = false;

	public void Set(Character c)
	{
		foreach (GameObject obj in createdObjects)
		{
			//Destroy(obj);
			obj.transform.SetParent(null);
		}
		objectsToDestroy = new List<GameObject>(createdObjects);
		Invoke("DestroyGarbage", .3f);
		createdObjects.Clear();

		StatusEffect[] effects = c.gameObject.GetComponents<StatusEffect>();
		foreach (StatusEffect effect in effects)
		{
			if (!effect.enabled)
			{
				continue;
			}

			GameObject obj = Instantiate(displayItemPrefab);
			createdObjects.Add(obj);
			obj.transform.SetParent(content);
			obj.GetComponent<StatusEffectDisplayItem>().Set(effect);
		}
	}

	private void DestroyGarbage()
	{
		foreach (GameObject obj in objectsToDestroy)
		{
			Destroy(obj);
		}
		awaitingDestroy = false;
		objectsToDestroy.Clear();
	}
}
