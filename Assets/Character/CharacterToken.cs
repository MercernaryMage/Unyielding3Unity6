using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterToken : MonoBehaviour
{
    public Character owningCharacter;
	MeshRenderer[] meshRenderers;
	public Vector3 UIPoint;

	private void Start()
	{
		meshRenderers = GetComponentsInChildren<MeshRenderer>();
		UIPoint = GetBonePosition("UIPoint");
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
