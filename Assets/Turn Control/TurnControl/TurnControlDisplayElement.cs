using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnControlDisplayElement : MonoBehaviour
{
	public TextMeshProUGUI value;
	public GameObject leftGroup;
	public GameObject rightArrow;
	public GameObject rightBar;
	public Image unitIcon;

	public void Set(int value, Sprite icon, bool left, bool rArrow, bool rBar)
	{
		this.value.text = value.ToString();
		unitIcon.sprite = icon;
		leftGroup.SetActive(left);
		rightArrow.SetActive(rArrow);
		rightBar.SetActive(rBar);
	}
}
