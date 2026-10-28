using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class GenericTooltipElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public string keyword;

	public void OnPointerEnter(PointerEventData eventData)
	{
		ExplanationIemScrptableObject explanation = ExplanationItemRepository.Instance.GetExplanation(keyword);
		GenericTooltip.Instance.Set(explanation.explanationName, explanation.explanationContent);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		GenericTooltip.Instance.Hide();
	}
}
