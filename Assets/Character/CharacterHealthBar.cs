using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterHealthBar : MonoBehaviour
{
	public Image healthBar;
	public Image reactionBar;
	public Image armorBar;
	Character character;

	public void Set(Character c)
	{
		character = c;
		if (!c.hero)
		{
			reactionBar.gameObject.SetActive(true);
		}
	}

	// Update is called once per frame
	void Update()
	{
		float t = character.currentHP / (float)character.maxHP;
		healthBar.rectTransform.sizeDelta = new Vector2(t * 100,
											healthBar.rectTransform.sizeDelta.y);

		if (character.characterDefinition.maxThreshold == -1)
		{
			t = 0;
		}
		else
		{
			t = character.threshold / (float)character.characterDefinition.maxThreshold;
			t = Mathf.Min(t, 1.0f);
		}
		reactionBar.rectTransform.sizeDelta = new Vector2(t * 100,
											reactionBar.rectTransform.sizeDelta.y);
		t = character.armor / (float)character.maxHP;
		t = Mathf.Min(t, 1.0f);
		armorBar.rectTransform.sizeDelta = new Vector2(t * 100,
											armorBar.rectTransform.sizeDelta.y);
	}
}
