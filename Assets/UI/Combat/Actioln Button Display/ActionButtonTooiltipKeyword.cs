using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ActionButtonTooiltipKeyword : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI textField;
	string keyword;

	public void OnPointerEnter(PointerEventData eventData)
	{
		ExplanationIemScrptableObject explanation = ExplanationItemRepository.Instance.GetExplanation(keyword);
		GenericTooltip.Instance.Set(explanation.explanationName, explanation.explanationContent);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		GenericTooltip.Instance.Hide();
	}

	public void Set(string s, string k)
	{
		textField.text = s;
		keyword = k;
	}
}
