using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextSetter : MonoBehaviour
{
    public TextMeshProUGUI bodyText;
	public TextMeshProUGUI titleText;
	public bool runCanvas = false;

	public void Set(string t)
	{
		bodyText.text = t;
		if (runCanvas)
		{
			Canvas.ForceUpdateCanvases();
			LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)bodyText.transform.parent);
		}
	}

	public void Set(string title, string body)
	{
		titleText.text = title;
		Set(body);
	}
}
