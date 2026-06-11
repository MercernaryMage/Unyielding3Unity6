using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterToken : MonoBehaviour
{
    public Character owningCharacter;
	MeshRenderer[] meshRenderers;

	private void Start()
	{
		meshRenderers = GetComponentsInChildren<MeshRenderer>();
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
}
