using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextFieldSetter : MonoBehaviour
{
    public TextMeshProUGUI field;
    public TextScriptableObject text;

	public void Start()
	{
		field.text = text.text;
	}
}
