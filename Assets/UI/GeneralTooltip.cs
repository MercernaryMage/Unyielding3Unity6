using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GeneralTooltip : MonoBehaviour
{
	public TextMeshProUGUI titleText;
	public TextMeshProUGUI bodyText;
	public GameObject bottomArrow;

	public void Set(string tt, string bt, bool showBottomArrow)
	{
		titleText.text = tt;
		bodyText.text = bt;
		bottomArrow.SetActive(showBottomArrow);
		Canvas.ForceUpdateCanvases();
		LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)bodyText.transform.parent);
	}
}
