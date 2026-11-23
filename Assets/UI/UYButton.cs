using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UYButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
	public bool genericHoverEffect = true;
	public UnityEvent onClick = new UnityEvent();

	Image image;

	void Awake()
	{
		if (genericHoverEffect)
		{
			image = GetComponent<Image>();
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.button != PointerEventData.InputButton.Left)
		{
			return;
		}
		onClick.Invoke();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		SoundManager.Instance.PlaySound("sfx_click");
		if (genericHoverEffect)
		{
			image.color = new Color(.8f, .8f, .8f);
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (genericHoverEffect)
		{
			image.color = new Color(1, 1, 1);
		}
	}
}
