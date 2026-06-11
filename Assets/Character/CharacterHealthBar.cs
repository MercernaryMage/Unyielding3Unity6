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
	public GameObject stunIcon;
	public GameObject slobberedIcon;
	public GameObject movementRemaining;
	public GameObject actionRemaining;
	public TextMeshProUGUI actionRemainingText;

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
		stunIcon.SetActive(character.GetComponent<Stun>() != null);
		slobberedIcon.SetActive(character.GetComponent<Slobbered>() != null);

		if (character.hero)
		{
			movementRemaining.SetActive(character.currentMovement > 0);
			actionRemaining.SetActive(character.actionCount > 0);
			actionRemainingText.text = character.actionCount.ToString();
		}
		else
		{
			movementRemaining.SetActive(false);
			actionRemaining.SetActive(false);
		}

		if (character.characterDefinition.maxThreshold == -1)
		{
			t = 0;
		}
		else
		{
			t = character.threshold / (float)character.characterDefinition.maxThreshold;
		}
		reactionBar.rectTransform.sizeDelta = new Vector2(t * 100,
											reactionBar.rectTransform.sizeDelta.y);
		t = character.armor / (float)character.maxHP;
		t = Mathf.Min(t, 1.0f);
		armorBar.rectTransform.sizeDelta = new Vector2(t * 100,
											armorBar.rectTransform.sizeDelta.y);
	}
}
