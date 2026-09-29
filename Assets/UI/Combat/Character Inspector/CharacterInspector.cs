using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInspector : SceneSingleton<CharacterInspector>
{
	public GameObject content;
	public GameObject characterEntryPrefab;
	public CharacterInspectorTraitDisplayGroup bottomUI;

	public TextMeshProUGUI characterName;

	public TextMeshProUGUI prowess;
	public TextMeshProUGUI cunning;

	public StatusEffectDisplayGroup statusEffectDisplayGroup;

	public RectTransform target;

	List<GameObject> createdObjects = new List<GameObject>();

	Character lastCharacter;

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
		if (c.hero)
		{
			SetHero(c);
		}
		else
		{
			SetEnemy(c);
		}

		Canvas.ForceUpdateCanvases();
		LayoutRebuilder.ForceRebuildLayoutImmediate(target);
		LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)target.parent);

		statusEffectDisplayGroup.Set(c);
	}

	void SetHero(Character c)
	{
		CreateEntry("Determination", c.storageCharacter.currentDetermination.ToString());
		CreateEntry("HP", $"{c.currentHP}/{c.maxHP}");
		CreateEntry("Armor", $"{c.armor}/{c.maxArmor}");
		CreateEntry("Evasion", $"{c.currentEvasion}");
		CreateEntry("Toughness", $"{c.toughness}");
		CreateEntry("Energy", $"{c.currentEnergy}/{c.characterDefinition.maxEnergy}");
		CreateEntry("Movement", $"{c.movementMax}");

		cunning.text = $"Cunning\n{c.characterDefinition.cunning}";
		prowess.text = $"Prowess\n{c.characterDefinition.prowess}";

		bottomUI.transform.SetAsLastSibling();
		bottomUI.ClearTraits();
		if (!PersistenceManager.Instance.GetFlag("WeaponSlotsLocked"))
		{
			bottomUI.Set(c.characterDefinition.traits);
		}
	}

	void SetEnemy(Character c)
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
		CreateEntry("Movement", $"{c.movementMax}");

		cunning.text = $"Cunning\n{c.characterDefinition.cunning}";
		prowess.text = $"Prowess\n{c.characterDefinition.prowess}";

		bottomUI.transform.SetAsLastSibling();
		bottomUI.ClearTraits();
		bottomUI.Set(c.characterDefinition.traits);
	}

	void CreateEntry(string leftText, string rightText)
	{
		GameObject obj = Instantiate(characterEntryPrefab);
		obj.GetComponent<CharacterInspectorEntry>().Set(leftText, rightText);
		obj.transform.SetParent(target);
		obj.transform.localScale = Vector3.one;
		createdObjects.Add(obj);
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
}
