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

	public GameObject leftArrow;
	public GameObject rightArrow;

	public GameObject leftPole;
	public GameObject rightPole;

	public void Set(StatusEffect effect)
	{
		icon.sprite = StatusEffectIconRepository.Instance.GetExactIcon(effect.GetIconName());
		titleText.text = effect.GetExplanationName();
		bodyText.text = effect.GetExplanation().explanationContent;
	}

	public void SetEnds(bool first, bool last)
	{
		leftArrow.SetActive(first);
		leftPole.SetActive(first);
		rightArrow.SetActive(last);
		rightPole.SetActive(true);
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
