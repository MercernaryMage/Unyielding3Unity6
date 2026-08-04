using System.Collections.Generic;
using UnityEngine;

public class EmptyTile : MonoBehaviour
{
	public List<GameObject> currentTileObjects ;
	public SwatchScriptableObject swatch;
	public int x;
	public int y;

	public void OnMouseDown()
	{
		LevelEditorManager.Instance.TileWasClicked(gameObject);
	}

	public void ChangeToTile(SwatchScriptableObject newSwatch)
	{
		foreach (GameObject obj in currentTileObjects)
		{
			Destroy(obj);
		}
		currentTileObjects.Clear();
		swatch = newSwatch;
		AddObject(swatch.prefab0);
		AddObject(swatch.prefab1);
		AddObject(swatch.prefab2);
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
