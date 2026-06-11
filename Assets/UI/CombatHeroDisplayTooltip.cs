using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CombatHeroDisplayTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject tooltipObject;

	public void OnPointerEnter(PointerEventData eventData)
	{
		tooltipObject.SetActive(true);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		tooltipObject.SetActive(false);
	}
}
