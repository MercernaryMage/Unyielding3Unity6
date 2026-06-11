using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TurnControlDisplayElement : MonoBehaviour
{
    public TextMeshProUGUI characterName;
	public TextMeshProUGUI value;

	public void Set(string displayName, int value)
	{
		characterName.text = displayName;
		this.value.text = value.ToString();
	}
}
