using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PrefabWeights
{
	public int weight;
	public GameObject prefab;
}

public class AutoDecorator : MonoBehaviour
{
	Tile ourTile;
	public List<PrefabWeights> prefabWeights;

	GameObject GetPrefab()
	{
		int total = 0;
		for (int i = 0; i < prefabWeights.Count; ++i)
		{
			total += prefabWeights[i].weight;
		}

		int index = Random.Range(0, total);
		int currentTotal = 0;
		for (int i = 0; i < prefabWeights.Count; ++i)
		{
			if (currentTotal + prefabWeights[i].weight > index)
			{
				return prefabWeights[i].prefab;
			}
			currentTotal += prefabWeights[i].weight;
		}

		return null;
	}

	private void Awake()
	{
		ourTile = GetComponent<Tile>();
	}

	void Start()
	{
		GameObject prefab = GetPrefab();

		if (prefab == null)
		{
			return;
		}

		GameObject obj = Instantiate(prefab);

		obj.transform.SetParent(transform, false);
		obj.transform.localPosition = Vector3.zero;
		float x = 1 / obj.transform.lossyScale.x;
		float y = 1 / obj.transform.lossyScale.y;
		float z = 1 / obj.transform.lossyScale.z;
		obj.transform.localScale = new Vector3(x, y, z);
		obj.transform.localPosition = new Vector3(0, .75f, 0);
		int index = Random.Range(0, 4);
		if (index == 0)
		{
			obj.transform.eulerAngles = new Vector3(0, 0, 0);
		}
		else if (index == 1)
		{
			obj.transform.eulerAngles = new Vector3(0, 90, 0);
		}
		else if (index == 0)
		{
			obj.transform.eulerAngles = new Vector3(0, 180, 0);
		}
		else
		{
			obj.transform.eulerAngles = new Vector3(0, 270, 0);
		}
		ourTile.ownedChildren.Add(obj);
	}
}
