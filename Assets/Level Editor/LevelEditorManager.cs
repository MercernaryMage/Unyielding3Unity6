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

	public SwatchScriptableObject currentSwatch;

	public Transform swatchTarget;
	public GameObject swatchPrefab;
	public GameObject swatchesObject;

	public float cameraSpeed = 15;
	public float zoomSpeed = 4;
	public float minOrthographicSize = 3;
	public float maxOrthographicSize = 30;

	public List<GameObject> tiles = new List<GameObject>();
	float tileScale = 1.5f;
	float cameraHeight;
	public int mapWidth;
	public int mapHeight;

	private void Start()
	{
		cameraHeight = Camera.main.transform.position.y;

		foreach (SwatchScriptableObject swatch in SwatchRepository.Instance.GetSwatches())
		{
			GameObject obj = Instantiate(swatchPrefab);
			obj.GetComponent<SwatchDisplay>().Set(swatch);
			obj.transform.SetParent(swatchTarget);
		}
	}

	private void Update()
	{
		Vector3 move = Vector3.zero;

		if (Input.GetKey(KeyCode.W))
		{
			move += Camera.main.transform.up;
		}
		if (Input.GetKey(KeyCode.S))
		{
			move -= Camera.main.transform.up;
		}
		if (Input.GetKey(KeyCode.D))
		{
			move += Camera.main.transform.right;
		}
		if (Input.GetKey(KeyCode.A))
		{
			move -= Camera.main.transform.right;
		}

		if (move != Vector3.zero)
		{
			Camera.main.transform.position += move * Time.deltaTime * cameraSpeed;
			Camera.main.transform.position = new Vector3(Camera.main.transform.position.x,
														 cameraHeight,
														 Camera.main.transform.position.z);
		}

		float scroll = Input.GetAxis("Mouse ScrollWheel");
		if (scroll != 0)
		{
			Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - scroll * zoomSpeed,
													   minOrthographicSize,
													   maxOrthographicSize);
		}
	}

	public void ClickGenerate()
	{
		Generate(System.Convert.ToInt32(XDimension.text), System.Convert.ToInt32(YDimension.text));
	}

	public void Generate(int xDim, int yDim)
	{
		foreach (GameObject tile in tiles)
		{
			Destroy(tile);
		}
		tiles.Clear();

		mapWidth = xDim;
		mapHeight = yDim;
		XDimension.text = xDim.ToString();
		YDimension.text = yDim.ToString();

		for (int y = 0; y < yDim; ++y)
		{
			for (int x = 0; x < xDim; ++x)
			{
				GameObject obj =  Instantiate(emptyTilePrefab);
				obj.transform.SetParent(combatWorld.transform);
				obj.name = $"{x}, {y}";


				obj.transform.localPosition = new Vector3(x * (tileScale + .75f), 0, y * (tileScale + .75f));
				obj.transform.localScale = Vector3.one * tileScale;

				EmptyTile emptyTile = obj.GetComponent<EmptyTile>();
				emptyTile.x = x;
				emptyTile.y = y;

				tiles.Add(obj);
			}
		}
		combatWorld.transform.localPosition = new Vector3(-2.5f, 0, -12.5f);
	}

	public void TileWasClicked(GameObject clickedGameObject)
	{
		if (Util.IsPointerOverUI())
		{
			return;
		}

		EmptyTile emptyTile = clickedGameObject.GetComponent<EmptyTile>();
		emptyTile.ChangeToTile(currentSwatch);
	}

	public void SwatchesClicked()
	{
		swatchesObject.SetActive(!swatchesObject.activeInHierarchy);
	}
}
