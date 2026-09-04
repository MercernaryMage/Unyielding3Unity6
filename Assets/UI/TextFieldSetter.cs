using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextFieldSetter : MonoBehaviour
{
    public TextMeshProUGUI field;
    public TextScriptableObject text;

	public void Start()
	{
		field.text = text.text;
		Canvas.ForceUpdateCanvases();
		LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)field.transform.parent);
	}
}
