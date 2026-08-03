using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class LevelEditorManager : SceneSingleton<LevelEditorManager>
{
	public GameObject emptyTilePrefab;
	public TMP_InputField XDimension;
	public TMP_InputField YDimension;

	public GameObject combatWorld;

	public GameObject debugTile;

	List<GameObject> tiles = new List<GameObject>();
	float tileScale = 1.5f;

	public void ClickGenerate()
	{
		int xDim = System.Convert.ToInt32(XDimension.text);
		int yDim = System.Convert.ToInt32(YDimension.text);

		for (int y = 0; y < yDim; ++y)
		{
			for (int x = 0; x < xDim; ++x)
			{
				GameObject obj =  Instantiate(emptyTilePrefab);
				obj.transform.SetParent(combatWorld.transform);
				obj.name = $"{x}, {y}";


				obj.transform.localPosition = new Vector3(x * (tileScale + .75f), 0, y * (tileScale + .75f));
				obj.transform.localScale = Vector3.one * tileScale;

				
				
				tiles.Add(obj);
			}
		}
		combatWorld.transform.localPosition = new Vector3(-2.5f, 0, -12.5f);
	}

	public void TileWasClicked(GameObject clickedGameObject)
	{
		EmptyTile emptyTile = clickedGameObject.GetComponent<EmptyTile>();
		emptyTile.ChangeToTile(debugTile);
	}
}
