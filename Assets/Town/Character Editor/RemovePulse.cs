using UnityEngine;

public class RemovePulse : MonoBehaviour
{
    public Pulse pulse;
	public RectTransform target;

	void Awake()
	{
		if (target == null)
		{
			target = transform as RectTransform;
		}
	}

	void Update()
	{
		if (!Input.GetMouseButtonUp(0))
		{
			return;
		}
		if (!IsMouseOver())
		{
			return;
		}

		Clicked();
	}

	public bool IsMouseOver()
	{
		if (target == null)
		{
			return false;
		}
		return RectTransformUtility.RectangleContainsScreenPoint(target, Input.mousePosition, null);
	}

	void Clicked()
	{
        transform.localScale = Vector3.one;
        Destroy(this);
        Destroy(pulse);
	}
}
