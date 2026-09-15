using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TMPMatInstance : MonoBehaviour
{
	public TextMeshProUGUI textMeshProUGUI;

	public bool overrideOutlineWidth = false;
	public float outlineWidth = 0;

	public void Start()
	{
		textMeshProUGUI.material = new Material(textMeshProUGUI.material);
		if (overrideOutlineWidth)
		{
			textMeshProUGUI.outlineWidth = outlineWidth;
		}
	}
}
