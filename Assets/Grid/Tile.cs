using JetBrains.Annotations;
using NUnit.Framework.Internal;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using static Tile;

public class Tile : MonoBehaviour
{
	public enum OverlayType
	{
		PossibleMovement,
		Selected,
		PossibleAttck,
		BlockedLOS
	}

	public TileScriptableObject tileScriptableObject;
	public GameObject model;
	public Character character;
	public Guest guest;
	public int x;
	public int y;
	public List<GameObject> ownedChildren = new List<GameObject>();
	Overlay overlay;
	HashSet<OverlayType> activeOverlays = new HashSet<OverlayType>();
	public List<GameObject> decos = new List<GameObject>();
	public GameObject foundation;
	public GameObject outline;

	void Start()
	{
		if (overlay != null)
		{
			overlay.TurnOffOverlay();
		}
		activeOverlays.Clear();
	}

	public void Init(int x, int y, GameObject m, GameObject o)
	{
		this.x = x;
		this.y = y;
		model = m;
		overlay = o.GetComponent<Overlay>();
		overlay.TurnOffOverlay();
	}

	public void EnterTile(Character character)
	{
		this.character = character;
	}

	public void PlaceInTile(Character character)
	{
		character.token.transform.SetParent(transform.parent);
		character.token.transform.localPosition =
			new Vector3(character.characterDefinition.size - 1,
						0,
						character.characterDefinition.size - 1);
	}

	void RefreshDisplay()
	{
		if (activeOverlays.Contains(OverlayType.BlockedLOS))
		{
			overlay.Set(OverlayType.BlockedLOS);
			return;
		}
		foreach (OverlayType type in activeOverlays)
		{
			overlay.Set(type);
			return;
		}
	}

	public void ShadeChildren(bool shade)
	{
		foreach (GameObject obj in decos)
		{
			MeshRenderer[] renderers = obj.GetComponentsInChildren<MeshRenderer>();
			foreach (MeshRenderer renderer in renderers)
			{
				MaterialPropertyBlock block = new MaterialPropertyBlock();
				renderer.GetPropertyBlock(block);
				if (shade)
				{
					block.SetFloat("_Darkened", 1);
				}
				else
				{
					block.SetFloat("_Darkened", 0);
				}
				renderer.SetPropertyBlock(block);
			}
		}
	}

	public void ShowOverlay(OverlayType overlayType)
	{
		activeOverlays.Add(overlayType);
		RefreshDisplay();
		foreach (GameObject child in ownedChildren)
		{
			child.GetComponent<DecoHider>()?.Hide();
		}
		BattleController.Instance.RequestRunDuckers();
	}

	public void HideOverlay(OverlayType overlayType)
	{
		activeOverlays.Remove(overlayType);
		if (activeOverlays.Count > 0)
		{
			RefreshDisplay();
		}
		else
		{
			overlay.TurnOffOverlay();
			foreach (GameObject child in ownedChildren)
			{
				child.GetComponent<DecoHider>()?.Show();
			}
		}
		BattleController.Instance.RequestRunDuckers();
	}

	public void HideAllOverlays()
	{
		activeOverlays.Clear();
		overlay.TurnOffOverlay();
		foreach (GameObject child in ownedChildren)
		{
			child.GetComponent<DecoHider>()?.Show();
		}
		BattleController.Instance.RequestRunDuckers();
	}

	public void OnMouseDown()
	{
		SelectionManager.Instance.TileClicked(this);
	}

	public void MouseEnterTile()
	{
		SelectionManager.Instance.MouseEnterTile(this);
	}

	public void MouseExitTile()
	{
		SelectionManager.Instance.MouseExitTile(this);
	}

	public void PerformDuckCheck()
	{
		if (character == null && activeOverlays.Count == 0)
		{
			return;
		}
		float c = .95f;
		Vector3[] offsets = new Vector3[]
							{
								new Vector3(c, 0, c),
								new Vector3(-c, 0, c),
								new Vector3(c, 0, -c),
								new Vector3(-c, 0, -c),
							};
		for (int i = 0; i < 4; ++i)
		{
			Vector3 pos = transform.position + offsets[i];
			RaycastHit[] hits = Physics.RaycastAll(pos, Camera.main.transform.position - pos);
			foreach (RaycastHit hit in hits)
			{
				PropDucker ducker = hit.collider.gameObject.GetComponent<PropDucker>();
				if (ducker != null)
				{
					ducker.Duck();
				}
			}
		}
	}

	public void SetSkirt(bool north, bool south, bool west, bool east)
	{
		MeshRenderer meshRenderer = foundation.GetComponent<MeshRenderer>();
		meshRenderer.material.SetFloat("_North", north ? 1 : 0);
		meshRenderer.material.SetFloat("_South", south ? 1 : 0);
		meshRenderer.material.SetFloat("_West", west ? 1 : 0);
		meshRenderer.material.SetFloat("_East", east ? 1 : 0);
	}

	public void ClearOutline()
	{
		MeshRenderer meshRenderer = outline.GetComponent<MeshRenderer>();
		meshRenderer.material.SetFloat("_FrontFace", 0);
		meshRenderer.material.SetFloat("_BackFace", 0);
		meshRenderer.material.SetFloat("_LeftFace", 0);
		meshRenderer.material.SetFloat("_RightFace", 0);
	}

	public void SetOutline(bool north, bool south, bool west, bool east, Color color)
	{
		MeshRenderer meshRenderer = outline.GetComponent<MeshRenderer>();
		meshRenderer.material.SetFloat("_FrontFace", north ? 1 : 0);
		meshRenderer.material.SetFloat("_BackFace", south ? 1 : 0);
		meshRenderer.material.SetFloat("_LeftFace", west ? 1 : 0);
		meshRenderer.material.SetFloat("_RightFace", east ? 1 : 0);
		meshRenderer.material.SetColor("_BaseColor", color);
	}
}
