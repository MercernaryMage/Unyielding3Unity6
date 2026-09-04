using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterInspector : SceneSingleton<CharacterInspector>
{
	public GameObject characterEntryPrefab;
	public CharacterInspectorTraitDisplayGroup bottomUI;

	public TextMeshProUGUI characterName;

	public TextMeshProUGUI prowess;
	public TextMeshProUGUI cunning;


	public RectTransform target;

	List<GameObject> createdObjects = new List<GameObject>();

	public void Set(Character c)
	{
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
}
