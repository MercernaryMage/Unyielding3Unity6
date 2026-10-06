using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StatusEffectDisplayItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	bool rotate;
    public Image icon;

	public GameObject explanationObject;

	public TextMeshProUGUI titleText;
	public TextMeshProUGUI bodyText;

	public GameObject leftArrow;
	public GameObject rightArrow;

	public GameObject leftPole;
	public GameObject rightPole;

	public void Set(StatusEffect effect, bool rotate)
	{
		icon.sprite = StatusEffectIconRepository.Instance.GetExactIcon(effect.GetIconName());
		titleText.text = effect.GetExplanationName();
		bodyText.text = effect.GetExplanation().explanationContent;
		this.rotate = rotate;
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
		RebuildExplanationLayout();
		if (rotate)
		{
			icon.transform.localRotation = Quaternion.Euler(0, 0, 90);
			explanationObject.transform.localRotation = Quaternion.Euler(0, 0, 90);
			explanationObject.transform.localPosition = new Vector3(((RectTransform)explanationObject.transform).sizeDelta.y - 53.0f, -405.6f, 0);
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		explanationObject.SetActive(false);
	}

	void RebuildExplanationLayout()
	{
		titleText.ForceMeshUpdate();
		bodyText.ForceMeshUpdate();

		RectTransform[] rects = explanationObject.GetComponentsInChildren<RectTransform>(true);
		for (int i = rects.Length - 1; i >= 0; --i)
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(rects[i]);
		}
	}
}
