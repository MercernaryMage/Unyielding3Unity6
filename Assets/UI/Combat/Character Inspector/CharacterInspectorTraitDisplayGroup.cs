using System.Collections.Generic;
using UnityEngine;

public class CharacterInspectorTraitDisplayGroup : MonoBehaviour
{
    public GameObject elementPrefab;
    public Transform target;

	List<GameObject> createdObjects = new List<GameObject>();

	public void Set(List<TraitScriptableObject> traits)
	{
		foreach (GameObject obj in createdObjects)
		{
			Destroy(obj);
		}
		createdObjects.Clear();

		foreach (TraitScriptableObject trait in traits)
		{
			GameObject obj = Instantiate(elementPrefab);
			obj.GetComponent<TextSetter>().Set(trait.displayName, trait.description);
			obj.transform.SetParent(target);
			obj.transform.localScale = Vector3.one;
			createdObjects.Add(obj);
		}
	}
}
