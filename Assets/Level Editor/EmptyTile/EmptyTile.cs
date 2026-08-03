using JetBrains.Annotations;
using UnityEngine;

public class EmptyTile : MonoBehaviour
{
	public GameObject currentTile;

	public void OnMouseDown()
	{
		LevelEditorManager.Instance.TileWasClicked(gameObject);
	}

	public void ChangeToTile(GameObject newTile)
	{
		Destroy(currentTile);
		GameObject newObj =  Instantiate(newTile);
		newObj.transform.SetParent(transform);
		newObj.transform.localPosition = Vector3.zero;
		newObj.transform.localScale = new Vector3(1, .1f, 1);
		currentTile = newObj;
	}
}
