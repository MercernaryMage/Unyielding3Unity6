using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StatusEffectDisplayItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image icon;

	public GameObject explanationObject;

	public TextMeshProUGUI titleText;
	public TextMeshProUGUI bodyText;

	public void Set(StatusEffect effect)
	{
		icon.sprite = StatusEffectIconRepository.Instance.GetExactIcon(effect.GetIconName());
		titleText.text = effect.GetDisplayName();
		bodyText.text = effect.GetEffectText(); ;
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		explanationObject.SetActive(true);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		explanationObject.SetActive(false);
	}
}
