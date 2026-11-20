using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInspector : SceneSingleton<CharacterInspector>
{
	public GameObject content;
	public GameObject characterEntryPrefab;
	public CharacterInspectorTraitDisplayGroup bottomUI;
	
	public GameObject characterContextButton;
	public TextMeshProUGUI contextButtonText;

	public TextMeshProUGUI characterName;

	public TextMeshProUGUI prowess;
	public TextMeshProUGUI cunning;

	public StatusEffectDisplayGroup statusEffectDisplayGroup;

	public RectTransform target;

	List<GameObject> createdObjects = new List<GameObject>();

	public Character lastCharacter;

	public TownCharacterDisplay townCharacterDisplay;
	public GameObject townCharacterDisplayContent;


	public void Set(Character c)
	{
		if (c == null)
		{
			return;
		}
		lastCharacter = c;
		Show();
		foreach (GameObject obj in createdObjects)
		{
			Destroy(obj);
			obj.transform.SetParent(null);
		}
		createdObjects.Clear();

		characterName.text = c.name;

		GameObject bottomObj;

		if (c.hero)
		{
			bottomObj = SetHero(c);
			characterContextButton.SetActive(true);
			contextButtonText.text = "View\nCharacter";
		}
		else
		{
			bottomObj = SetEnemy(c);
			characterContextButton.SetActive(false);
			contextButtonText.text = "View Deck";
		}

		Canvas.ForceUpdateCanvases();
		LayoutRebuilder.ForceRebuildLayoutImmediate(target);
		LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)target.parent);

		characterContextButton.transform.position = new Vector3(
													characterContextButton.transform.position.x,
													bottomObj.transform.position.y - 40,
													characterContextButton.transform.position.z);

		statusEffectDisplayGroup.Set(c);
	}

	GameObject SetHero(Character c)
	{
		CreateEntry("Determination", c.storageCharacter.currentDetermination.ToString());
		CreateEntry("HP", $"{c.currentHP}/{c.maxHP}");
		CreateEntry("Armor", $"{c.armor}/{c.maxArmor}");
		CreateEntry("Evasion", $"{c.currentEvasion}");
		CreateEntry("Toughness", $"{c.toughness}");
		CreateEntry("Energy", $"{c.currentEnergy}/{c.characterDefinition.maxEnergy}");
		GameObject obj = CreateEntry("Movement", $"{c.movementMax}");

		cunning.text = $"Cunning\n{c.characterDefinition.cunning}";
		prowess.text = $"Prowess\n{c.characterDefinition.prowess}";

		bottomUI.transform.SetAsLastSibling();
		bottomUI.ClearTraits();
		if (!PersistenceManager.Instance.GetFlag("WeaponSlotsLocked"))
		{
			bottomUI.Set(c.characterDefinition.traits);
		}
		return obj;
	}

	GameObject SetEnemy(Character c)
	{
		CreateEntry("HP", $"{c.currentHP}/{c.maxHP}");
		CreateEntry("Armor", $"{c.armor}/{c.maxArmor}");
		CreateEntry("Evasion", $"{c.currentEvasion}");
		CreateEntry("Toughness", $"{c.toughness}");
		if (c.characterDefinition.maxThreshold == -1)
		{
			CreateEntry("Reaction", $"-/-");
		}
		else
		{
			CreateEntry("Reaction", $"{c.threshold}/{c.characterDefinition.maxThreshold}");
		}
		GameObject obj = CreateEntry("Movement", $"{c.movementMax}");

		Canvas.ForceUpdateCanvases();
		LayoutRebuilder.ForceRebuildLayoutImmediate(target);
		LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)target.parent);

		cunning.text = $"Cunning\n{c.characterDefinition.cunning}";
		prowess.text = $"Prowess\n{c.characterDefinition.prowess}";

		bottomUI.transform.SetAsLastSibling();
		bottomUI.ClearTraits();
		bottomUI.Set(c.characterDefinition.traits);
		return obj;
	}

	GameObject CreateEntry(string leftText, string rightText)
	{
		GameObject obj = Instantiate(characterEntryPrefab);
		obj.GetComponent<CharacterInspectorEntry>().Set(leftText, rightText);
		obj.transform.SetParent(target);
		obj.transform.localScale = Vector3.one;
		createdObjects.Add(obj);
		return obj;
	}

	public void Show()
	{
		content.SetActive(true);
	}

	public void Hide()
	{
		content.SetActive(false);
	}

	public void Refresh()
	{
		if (content.activeInHierarchy)
		{
			Set(lastCharacter);
		}
	}

	public void DisplayCharaccterClicked()
	{
		if (lastCharacter != null && lastCharacter.storageCharacter != null)
		{
			townCharacterDisplayContent.SetActive(true);
			townCharacterDisplay.SetToCharacter(lastCharacter.storageCharacter, null);
		}
	}
}
