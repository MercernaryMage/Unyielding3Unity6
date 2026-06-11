using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class FancyHeroDisplay : MonoBehaviour
{
	public GameObject actionButtonPrefab;

	public GameObject content;

	public TextMeshProUGUI determination;
	public TextMeshProUGUI characterName;
	public TextMeshProUGUI hp;
	public TextMeshProUGUI armor;
	public TextMeshProUGUI actionPoint;
	public TextMeshProUGUI energy;
	public TextMeshProUGUI movement;
	public TextMeshProUGUI toughness;
	public TextMeshProUGUI evasion;

	public Image characterFace;

	public Character lastCharacter;
	public bool showing = false;

	List<GameObject> createdObjects = new List<GameObject>();

	public Transform target;

	public StatusEffectDisplayGroup statusEffectDisplayGroup;

	public void Set(Character c)
	{
		lastCharacter = c;
		characterFace.sprite = c.characterDefinition.battlePortrait;

		//Create lower UI
		CreateButtons();
		statusEffectDisplayGroup.Set(c);
	}

	void CreateButtons()
	{
		foreach (GameObject obj in createdObjects)
		{
			Destroy(obj);
		}
		createdObjects.Clear();

		if (lastCharacter.GetComponent<Stun>() || lastCharacter.GetComponent<Downed>())
		{
			return;
		}

		List<Item> allItems = new List<Item>(lastCharacter.storageCharacter.equipment);
		allItems.AddRange(lastCharacter.temporaryItems);
		foreach (Item item in allItems)
		{
			for (int i = 0; i < item.itemDefinition.actions.Count; ++i)
			{
				ActionPattern pattern = item.itemDefinition.actions[i];
				if (!HeroDisplay.IsActionDisplayable(lastCharacter, item, pattern))
				{
					continue;
				}
				/*if (pattern.uniqueName != "" && createdObjects.Any(o => o.uniqueName == pattern.uniqueName))
				{
					continue;
				}*/
				GameObject obj = Instantiate(actionButtonPrefab);
				obj.transform.SetParent(target);
				Tuple<bool, string> usable = HeroDisplay.IsActionUsable(lastCharacter, item, pattern);
				ActionButtonDisplay display = obj.GetComponent<ActionButtonDisplay>();
				obj.GetComponent<ActionButtonDisplay>().Set(lastCharacter, pattern, item, usable.Item1, usable.Item2, i);
				createdObjects.Add(obj);
			}
		}
	}

	public void Show()
	{
		showing = true;
		content.SetActive(true);
	}

	public void Hide(bool hideMovement)
	{
		showing = false;
		content.SetActive(false);
		if (hideMovement)
		{
			MovementController.Instance.HideMovement();
		}
	}

	public void UpdateWithLastCharacter()
	{
		if (lastCharacter == null)
		{
			return;
		}

		if (lastCharacter.currentMovement > 0 && lastCharacter.GetComponent<Downed>() == false &&
			BattleController.playerHasControl)
		{
			MovementController.Instance.ShowMovement(lastCharacter);
		}
		Show();
		Set(lastCharacter);
	}

	void Update()
	{
		if (lastCharacter == null)
		{
			return;
		}

		characterName.text = lastCharacter.displayName;
		hp.text = $"{lastCharacter.currentHP}/{lastCharacter.maxHP}";
		determination.text = $"{lastCharacter.storageCharacter.currentDetermination}";
		armor.text = $"{lastCharacter.armor}";
		actionPoint.text = $"{lastCharacter.actionCount}";
		energy.text = $"{lastCharacter.currentEnergy}/{lastCharacter.characterDefinition.maxEnergy}";
		movement.text = $"{lastCharacter.currentMovement}";
		toughness.text = $"{lastCharacter.toughness}";
		evasion.text = $"{lastCharacter.characterDefinition.evasion}";
	}

	
}
