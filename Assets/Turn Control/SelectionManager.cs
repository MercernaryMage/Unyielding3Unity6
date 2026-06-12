using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectionManager : SceneSingleton<SelectionManager>
{
	public float scrollSpeed = 2;
	static float root2 = 1.4142f;

	List<Tile> losTiles = new List<Tile>();
	List<Character> losBlockedCharacters = new List<Character>();
	Tile LOSTile = null;
	public float testSpeed = 5;
	Vector3 lastMousePos;
	Vector3 clickVec;
	bool panAllowed;
	bool clickedUI = false;
	bool cameraMoving = false;
	bool cancelClicked = false;

	public GameObject cancelButton;

	public void TileClicked(Tile t)
	{
		List<RaycastResult> results = GetUIRaycast();
		if (results.Count > 0)
		{
			return;
		}

		if (t.character != null)
		{
			UIController.Instance.ShowCharacter(t.character);
		}
	}

	List<RaycastResult> GetUIRaycast()
	{
		PointerEventData pointerData = new PointerEventData(EventSystem.current);
		pointerData.position = Input.mousePosition;

		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(pointerData, results);
		return results;
	}

	void RotateCamera(float angle)
	{
		RotateAround rotate = Camera.main.AddComponent<RotateAround>();
		rotate.angle = angle;
		float theta = Vector3.Angle(Camera.main.transform.forward, Vector3.down);
		float mag = Camera.main.transform.position.y / Mathf.Sin(theta);
		rotate.rotateAroundPoint = Camera.main.transform.position + Camera.main.transform.forward * mag;
		rotate.rotateAroundAxis = Vector3.up;
	}

	public void Update()
	{
		if (cancelClicked)
		{
			cancelClicked = false;
			return;
		}
		if (Input.GetMouseButtonDown(0))
		{
			clickVec = Input.mousePosition;
			lastMousePos = Input.mousePosition;
			panAllowed = GetUIRaycast().Count == 0;
		}
		//why 17.14f?  It's just some arbitrary height above the ground plane to make sure we don't clip into th ground.
		if (Input.GetMouseButton(0) && panAllowed && !cameraMoving)
		{
			Vector3 mouseDelta = Input.mousePosition - lastMousePos;
			lastMousePos = Input.mousePosition;
			if (mouseDelta.magnitude > 0)
			{
				float scrollScaleX = 10 / Camera.main.orthographicSize * 55;
				float scrollScaleY = 10 / Camera.main.orthographicSize * 27;
				Camera.main.transform.position += Camera.main.transform.right * (-mouseDelta.x / scrollScaleX);
				Camera.main.transform.position += Camera.main.transform.up * (-mouseDelta.y / scrollScaleY);
				Camera.main.transform.position = new Vector3(Camera.main.transform.position.x,
															 17.14f,
															 Camera.main.transform.position.z);
			}
		}
		if (Input.GetKey(KeyCode.A) && !cameraMoving)
		{
			Camera.main.transform.position += -Camera.main.transform.right * Time.deltaTime * scrollSpeed;
			Camera.main.transform.position = new Vector3(Camera.main.transform.position.x,
														 17.14f,
														 Camera.main.transform.position.z);
		}
		if (Input.GetKey(KeyCode.D) && !cameraMoving)
		{
			Camera.main.transform.position += Camera.main.transform.right * Time.deltaTime * scrollSpeed;
			Camera.main.transform.position = new Vector3(Camera.main.transform.position.x,
														 17.14f,
														 Camera.main.transform.position.z);
		}
		if (Input.GetKey(KeyCode.S) && !cameraMoving)
		{
			Camera.main.transform.position += -Camera.main.transform.up * Time.deltaTime * scrollSpeed * root2;
			Camera.main.transform.position = new Vector3(Camera.main.transform.position.x,
														 17.14f,
														 Camera.main.transform.position.z);
		}
		if (Input.GetKey(KeyCode.W) && !cameraMoving)
		{
			Camera.main.transform.position += Camera.main.transform.up * Time.deltaTime * scrollSpeed * root2;
			Camera.main.transform.position = new Vector3(Camera.main.transform.position.x,
														 17.14f,
														 Camera.main.transform.position.z);
		}
		if (Input.GetKeyDown(KeyCode.Q) && Camera.main.GetComponent<RotateAround>() == null && !cameraMoving)
		{
			RotateCamera(90);
		}
		if (Input.GetKeyDown(KeyCode.E) && Camera.main.GetComponent<RotateAround>() == null && !cameraMoving)
		{
			RotateCamera(-90);
		}

		float scroll = Input.GetAxis("Mouse ScrollWheel");
		if (scroll != 0f)
		{
			scroll *= 4;
			Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - scroll, 3f, 10f);
		}

		if (Input.GetMouseButton(2))
		{
			Tile newLOSTile = GetMousedOverTile();
			if (LOSTile != newLOSTile)
			{
				ClearLOS();
			}

			LOSTile = newLOSTile;
			if (LOSTile != null)
			{
				LOSTile.ShowOverlay(Tile.OverlayType.Selected);
				if (LOSTile)
				{
					foreach (Tile t1 in TileGrid.Instance.tiles)
					{
						if (t1 == LOSTile)
						{
							continue;
						}
						if (!TileGrid.Instance.DoesTileHaveLOSToTile(LOSTile, t1))
						{
							losTiles.Add(t1);
							t1.ShowOverlay(Tile.OverlayType.BlockedLOS);
							t1.ShadeChildren(true);
						}
					}
					List<Character> allCharacters = new List<Character>(BattleController.Instance.heroes);
					allCharacters.AddRange(BattleController.Instance.enemies);
					foreach (Character c in allCharacters)
					{
						List<Tile> charTiles = TileGrid.Instance.FindCharacter(c);
						bool anyHasLOS = false;
						foreach (Tile charTile in charTiles)
						{
							if (TileGrid.Instance.DoesTileHaveLOSToTile(LOSTile, charTile))
							{
								anyHasLOS = true;
								break;
							}
						}
						if (!anyHasLOS)
						{
							losBlockedCharacters.Add(c);
							c.ShadeToken();
						}
					}
				}
			}
		}
		else if (Input.GetMouseButtonUp(2))
		{
			ClearLOS();
			Tile hoveredTile = GetMousedOverTile();
			if (hoveredTile != null)
			{
				MouseEnterTile(hoveredTile);
			}
		}

		if (Input.GetMouseButtonUp(0))
		{

			if ((clickVec - Input.mousePosition).magnitude > 3)
			{
				return;
			}


			List<RaycastResult> results = GetUIRaycast();

			if (results.Count > 0)
			{
				return;
			}

			if (MovementController.Instance.running)
			{
				if (MovementController.Instance.HandleClick())
				{
					return;
				}
			}
			else if (ActionController.Instance.running)
			{
				if (ActionController.Instance.HandleClick())
				{
					return;
				}
			}

			bool found = results.Count > 0;

			if (!found)
			{
				Tile t = GetMousedOverTile();
				if (t != null)
				{
					if (t.character == null)
					{
						UIController.Instance.NothingClicked();
					}
					found = true;
				}
			}
			if (!found)
			{
				UIController.Instance.NothingClicked();
			}
		}
		else if (Input.GetMouseButtonUp(1))
		{
			if (ActionController.Instance.running)
			{
				ActionController.Instance.HandleRightClick();
			}
		}
	}

	public void ShowCancelButton(bool active)
	{
		cancelButton.SetActive(active);
	}

	public void CancelClicked()
	{
		if (ActionController.Instance.running)
		{
			cancelClicked = true;
			ActionController.Instance.HandleRightClick();
		}
	}

	void ClearLOS()
	{
		if (LOSTile != null)
		{
			LOSTile.HideOverlay(Tile.OverlayType.Selected);
		}
		LOSTile = null;
		foreach (Tile t in losTiles)
		{
			t.HideOverlay(Tile.OverlayType.BlockedLOS);
			t.ShadeChildren(false);
		}
		losTiles.Clear();
		foreach (Character c in losBlockedCharacters)
		{
			c.UnshadeToken();
		}
		losBlockedCharacters.Clear();
	}

	public Tile GetMousedOverTile()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit[] hits = Physics.RaycastAll(ray);

		foreach (RaycastHit hit in hits)
		{
			Tile t = hit.collider.gameObject.GetComponent<Tile>();
			if (t != null)
			{
				return t;
			}
		}
		return null;
	}

	public void MouseEnterTile(Tile t)
	{
		MovementController.Instance.previousMousedTile = t;
		if (MovementController.Instance.running)
		{
			MovementController.Instance.MouseOverTile(t);
		}
		else if (ActionController.Instance.running)
		{
			ActionController.Instance.MouseOverTile(t);
		}
	}

	public void MouseExitTile(Tile t)
	{
		MovementController.Instance.MouseExitTile(t);
	}

	public float SnapCameraToCharacter(Character targetCharacter)
	{
		Vector3 tokenPos = targetCharacter.token.transform.position;
		Vector3 camForward = Camera.main.transform.forward;
		float s = (tokenPos.y - Camera.main.transform.position.y) / camForward.y;
		Vector3 finalPos = new Vector3(
			tokenPos.x - camForward.x * s,
			Camera.main.transform.position.y,
			tokenPos.z - camForward.z * s
		);

		GlobalPositionLerp globalPositionLerp = Camera.main.AddComponent<GlobalPositionLerp>();
		float runTime = (Camera.main.transform.position - finalPos).magnitude / 15.0f;
		globalPositionLerp.runTime = runTime;
		globalPositionLerp.p0 = Camera.main.transform.position;
		globalPositionLerp.p1 = finalPos;
		globalPositionLerp.type = InterpolationType.Smoother;
		cameraMoving = true;
		globalPositionLerp.callbackFunction = () =>
		{
			cameraMoving = false;
		};
		globalPositionLerp.Init();

		return runTime;
	}
}
