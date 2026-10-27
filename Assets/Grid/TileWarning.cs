using TMPro;
using UnityEngine;

public class TileWarning : MonoBehaviour
{
	public TextMeshProUGUI textDisplay;
	public RectTransform background;

	bool mouseOver;

	string tooltipTitle;
	string tooltipText;

	void Awake()
	{
		if (background == null)
		{
			Transform backgroundTransform = transform.Find("BG");
			if (backgroundTransform != null)
			{
				background = (RectTransform)backgroundTransform;
			}
		}
	}

	void Update()
	{
		bool nowOver = IsMouseOver();
		if (nowOver == mouseOver)
		{
			return;
		}

		mouseOver = nowOver;
		if (mouseOver)
		{
			MouseEntered();
		}
		else
		{
			MouseExited();
		}
	}

	public bool IsMouseOver()
	{
		if (background == null)
		{
			return false;
		}
		return RectTransformUtility.RectangleContainsScreenPoint(background, Input.mousePosition, null);
	}

	void MouseEntered()
	{
		GenericTooltip.Instance.Set(tooltipTitle, tooltipText);
	}

	void MouseExited()
	{
		GenericTooltip.Instance.Hide();
	}

	public void SetText(string text, string TTtitle, string TTtext)
	{
		tooltipTitle = TTtitle;
		tooltipText = TTtext;
		textDisplay.text = text;
	}
}
