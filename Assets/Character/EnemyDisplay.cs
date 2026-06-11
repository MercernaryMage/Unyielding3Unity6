using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyDisplay : SceneSingleton<EnemyDisplay>
{
	public GameObject content;

	public TextMeshProUGUI characterName;
	public TextMeshProUGUI toughnessText;
	public TextMeshProUGUI HPText;
	public TextMeshProUGUI armorText;
	public TextMeshProUGUI reactionText;
	public TextMeshProUGUI evasionText;

	Character currentCharacter;

	bool shouldCloseCard;

	public void Hide()
	{
		if (shouldCloseCard)
		{
			AICardDisplay.Instance.Dismiss();
		}
		content.SetActive(false);
	}

	public void Show()
	{
		content.SetActive(true);
	}

	private void Update()
	{
		if (currentCharacter == null || !content.activeInHierarchy)
		{
			return;
		}
		toughnessText.text = currentCharacter.toughness.ToString();
		HPText.text = $"{currentCharacter.currentHP}/{currentCharacter.maxHP}";
		armorText.text = $"{currentCharacter.armor}/{currentCharacter.maxArmor}";
		evasionText.text = currentCharacter.characterDefinition.evasion.ToString();
		if (currentCharacter.characterDefinition.maxThreshold == -1)
		{
			reactionText.text = $"-/-";
		}
		else
		{
			reactionText.text = $"{currentCharacter.threshold}/{currentCharacter.characterDefinition.maxThreshold}";
		}


		if (!AICardDisplay.Instance.isShowing && currentCharacter.cards[0].isRevealed && !ActionController.Instance.running)
		{
			shouldCloseCard = true;
			AICardDisplay.Instance.ShowCard(currentCharacter.cards[0].cardScriptableObject);
		}
	}

	public void Set(Character c)
	{
		currentCharacter = c;
		characterName.text = c.displayName;
		
	}
}
