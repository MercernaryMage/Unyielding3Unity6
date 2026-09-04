using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInspectorEntry : MonoBehaviour
{
	public float width = 340;
    public TextMeshProUGUI leftText;
	public TextMeshProUGUI rightText;
	public RectTransform middleObject;

	public void Set(string lt, string rt)
	{
		leftText.text = lt;
		rightText.text = rt;
		Canvas.ForceUpdateCanvases();
		LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)leftText.transform);
		LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)rightText.transform);
		float newWidth = width - 26 - leftText.rectTransform.rect.width - rightText.rectTransform.rect.width;
		middleObject.sizeDelta = new Vector2(newWidth, middleObject.sizeDelta.y);
		Canvas.ForceUpdateCanvases();
		LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)middleObject.transform);
	}
}
