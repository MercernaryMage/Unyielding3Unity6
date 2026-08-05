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

	public GameObject propsParent;

	public float doubleClickTime = .5f;

	bool paintAllowed;
	float lastGenerateClick = -100;
	EmptyTile lastPaintedTile;

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
		if (Input.GetMouseButtonDown(0))
		{
			paintAllowed = !Util.IsPointerOverUI();
			lastPaintedTile = null;
		}

		if (Input.GetMouseButton(0) && paintAllowed && !Util.IsPointerOverUI())
		{
			Paint();
		}

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
		if (tiles.Count > 0 && Time.time - lastGenerateClick > doubleClickTime)
		{
			lastGenerateClick = Time.time;
			Debug.Log("Click Generate again to replace the current map");
			return;
		}

		lastGenerateClick = -100;
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

	void Paint()
	{
		if (currentSwatch == null)
		{
			return;
		}

		RaycastHit hit;
		if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
		{
			return;
		}

		EmptyTile emptyTile = hit.collider.GetComponentInParent<EmptyTile>();
		if (emptyTile == null || emptyTile == lastPaintedTile)
		{
			return;
		}

		lastPaintedTile = emptyTile;
		emptyTile.ChangeToTile(currentSwatch);
	}

	public void SwatchesClicked()
	{
		swatchesObject.SetActive(!swatchesObject.activeInHierarchy);
	}
}
