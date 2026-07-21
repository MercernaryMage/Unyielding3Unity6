using TMPro;
using UnityEngine;

public class TileWarning : MonoBehaviour
{
	public TextMeshProUGUI textDisplay;

	public void SetText(string text)
	{
		textDisplay.text = text;
	}
}
