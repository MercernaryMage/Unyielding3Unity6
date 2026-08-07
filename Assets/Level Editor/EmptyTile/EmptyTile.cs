using System.Collections.Generic;
using UnityEngine;
using static MapParser;

public class EmptyTile : MonoBehaviour
{
	public List<GameObject> currentTileObjects ;
	public SwatchScriptableObject swatch;
	public int x;
	public int y;
	public int seed;

	public void ChangeToTile(SwatchScriptableObject newSwatch, bool newTile)
	{
		foreach (GameObject obj in currentTileObjects)
		{
			obj.transform.SetParent(null);
			Destroy(obj);
		}
		currentTileObjects.Clear();
		swatch = newSwatch;
		AddObject(swatch.prefab0);
		AddObject(swatch.prefab1);
		AddObject(swatch.prefab2);

		if (newTile)
		{
			seed = Random.Range(0, int.MaxValue);
		}

		System.Random rand = new System.Random(seed);
		AutoDecorator[] autoDecs = transform.GetComponentsInChildren<AutoDecorator>();
		foreach (AutoDecorator autoDec in autoDecs)
		{
			autoDec.Set(rand);
		}
	}

	void AddObject(GameObject obj)
	{
		if (obj == null)
		{
			return;
		}
		GameObject newObj = Instantiate(obj);
		newObj.transform.SetParent(transform);
		newObj.transform.localPosition = Vector3.zero;
		newObj.transform.localScale = new Vector3(1, 1, 1);
		foreach (Tile tile in newObj.GetComponentsInChildren<Tile>())
		{
			tile.enabled = false;
		}
		currentTileObjects.Add(newObj);
	}
}
