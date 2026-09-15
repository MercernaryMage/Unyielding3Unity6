using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TurnControlDisplayElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public TextMeshProUGUI value;
	public GameObject leftGroup;
	public GameObject rightArrow;
	public GameObject rightBar;
	public Image unitIcon;

	public Image frame;
	public Image background;

	public Sprite enemyBackground;
	public Sprite enemyFrame;

	public Character character;

	public void Set(Character character, int value, bool left, bool rArrow, bool rBar)
	{
		this.character = character;
        if (!character.hero)
        {
			frame.sprite = enemyFrame;
			background.sprite = enemyBackground;
        }
        this.value.text = value.ToString();
		unitIcon.sprite = character.characterDefinition.battlePortrait;
		leftGroup.SetActive(left);
		rightArrow.SetActive(rArrow);
		rightBar.SetActive(rBar);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (character != null && character.selectionVisual != null)
		{
			character.selectionVisual.SetActive(true);
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (character != null && character.selectionVisual != null)
		{
			character.selectionVisual.SetActive(false);
		}
	}
}
