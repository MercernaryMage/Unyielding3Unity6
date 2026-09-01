using UnityEngine;
using UnityEngine.UI;

public class UYMaskBar : MonoBehaviour
{
	public enum AxisOfMotion
	{
		Left,
		Down
	}
	public RectMask2D mask;
	public AxisOfMotion axisOfMotion;
	public float offset;

	RectTransform rect;

	void Awake()
	{
		rect = GetComponent<RectTransform>();
	}

	public void Set(float t)
	{
		t = Mathf.Clamp01(t);
		float value = 0;
		if (axisOfMotion == AxisOfMotion.Left)
		{
			value = (1 - t) * (rect.sizeDelta.x - offset);
			mask.padding = new Vector4(0, 0, value, 0);
		}
		else if (axisOfMotion == AxisOfMotion.Down)
		{
			value = (1 - t) * (rect.sizeDelta.y - offset);
			mask.padding = new Vector4(0, 0, 0, value);
		}
	}

}
