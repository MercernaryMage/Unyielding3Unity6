using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class OverlayFlicker : MonoBehaviour
{
	public List<GameObject> targets;

	float value;

	private void OnEnable()
	{
		value = 0;
		Apply();
	}

	private void OnDisable()
	{
		value = 0;
		Apply();
	}

	void Apply()
	{
		//_Color
		float t = Mathf.Cos(value * 4);

		Color color1 = Color.white;
		Color color2 = Color.red;

		Color finalColor = Color.Lerp(color1, color2, t);

		MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
		materialPropertyBlock.SetColor("_Color", finalColor);

		foreach (GameObject obj in targets)
		{
			obj.GetComponent<MeshRenderer>().SetPropertyBlock(materialPropertyBlock);
		}
	}

	void Update()
	{
		value += Time.deltaTime;
		Apply();
	}
}
