using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterToken : MonoBehaviour
{
    public Character owningCharacter;
	MeshRenderer[] meshRenderers;
	public Vector3 UIPoint;
	Transform uiPoint;
	public GameObject healthBar;
	public float referenceOrthographicSize = 10f;

	private void Start()
	{
		meshRenderers = GetComponentsInChildren<MeshRenderer>();
		uiPoint = transform.Find("UIPoint");
		UIPoint = uiPoint.position;
	}

	private void Update()
	{
		if (healthBar == null)
		{
			return;
		}
		Camera camera = Camera.main;
		healthBar.transform.position = camera.WorldToScreenPoint(uiPoint.position);
		float scale = referenceOrthographicSize / camera.orthographicSize;
		healthBar.transform.localScale = new Vector3(scale, scale, 1f);
	}

	private void OnDestroy()
	{
		if (healthBar != null)
		{
			Destroy(healthBar);
		}
	}

	public void Shade()
	{
		foreach (MeshRenderer renderer in meshRenderers)
		{
			MaterialPropertyBlock block = new MaterialPropertyBlock();
			renderer.GetPropertyBlock(block);
			block.SetFloat("_Darkened", 1);
			renderer.SetPropertyBlock(block);
		}
	}

	public void UnShade()
	{
		foreach (MeshRenderer renderer in meshRenderers)
		{
			MaterialPropertyBlock block = new MaterialPropertyBlock();
			renderer.GetPropertyBlock(block);
			block.SetFloat("_Darkened", 0);
			renderer.SetPropertyBlock(block);
		}
	}
	public Vector3 GetBonePosition(string str)
	{
		Transform t = transform.Find(str);
		if (t != null)
		{
			return t.position;
		}
		return Vector3.zero;
	}
}
