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

	List<List<GameObject>> createdObjects = new List<List<GameObject>>()
	{
		new List<GameObject>(),
		new List<GameObject>(),
		new List<GameObject>()
	};

	public Transform attackButtonTarget;
	public Transform advantagesButtonTarget;
	public Transform otherButtonTarget;

	public StatusEffectDisplayGroup statusEffectDisplayGroup;

	public UYMaskBar hpBar;
	public UYMaskBar armorBar;

	public UYMaskBar APBar;
	public UYMaskBar energyBar;
	public UYMaskBar movementBar;

	public void Set(Character c)
	{
		lastCharacter = c;
		characterFace.sprite = c.characterDefinition.battlePortrait;

		//Create lower UI
		CreateButtons();
		statusEffectDisplayGroup.Set(c);
		ImposedControl.Instance.Run();
	}

	void CreateButtons()
	{
		foreach (List<GameObject> list in createdObjects)
		{
			foreach (GameObject obj in list)
			{
				Destroy(obj);
			}
			list.Clear();
		}

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
				GetWarningForActionMessage getWarningForAction = new GetWarningForActionMessage();
				getWarningForAction.character = lastCharacter;
				getWarningForAction.actionPattern = pattern;
				MessagePump.Instance.SendMessage(getWarningForAction);
				if (!HeroDisplay.IsActionDisplayable(lastCharacter, item, pattern))
				{
					continue;
				}
				/*if (pattern.uniqueName != "" && createdObjects.Any(o => o.uniqueName == pattern.uniqueName))
				{
					continue;
				}*/

				//attack
				//advantage
				//other

				GameObject obj = Instantiate(actionButtonPrefab);
				if (pattern.attack)
				{
					obj.transform.SetParent(attackButtonTarget);
					obj.transform.transform.localScale = Vector3.one;
					createdObjects[0].Add(obj);
				}
				else if (!pattern.other)
				{
					obj.transform.SetParent(advantagesButtonTarget);
					obj.transform.transform.localScale = Vector3.one;
					createdObjects[1].Add(obj);
				}
				else
				{
					obj.transform.SetParent(otherButtonTarget);
					obj.transform.transform.localScale = Vector3.one;
					createdObjects[2].Add(obj);
				}

				Tuple<bool, string> usable = HeroDisplay.IsActionUsable(lastCharacter, item, pattern);
				ActionButtonDisplay display = obj.GetComponent<ActionButtonDisplay>();
				obj.GetComponent<ActionButtonDisplay>().Set(lastCharacter, pattern, item, usable.Item1, usable.Item2, i, getWarningForAction.warnings);
			}
		}
		Canvas.ForceUpdateCanvases();
		LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)attackButtonTarget.transform);
		LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)advantagesButtonTarget.transform);
		LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)otherButtonTarget.transform);
		for (int i = 0; i < createdObjects.Count; ++i)
		{
			if (i == 0)
			{
				attackButtonTarget.parent.gameObject.SetActive(createdObjects[i].Count > 0);
			}
			else if (i == 1)
			{
				advantagesButtonTarget.parent.gameObject.SetActive(createdObjects[i].Count > 0);
			}
			else
			{
				otherButtonTarget.parent.gameObject.SetActive(createdObjects[i].Count > 0);
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
		hpBar.Set(lastCharacter.currentHP / (float)lastCharacter.maxHP);
		determination.text = $"{lastCharacter.storageCharacter.currentDetermination}";
		armor.text = $"{lastCharacter.armor} / {lastCharacter.maxArmor}";
		armorBar.Set(lastCharacter.armor / (float)lastCharacter.maxArmor);
		actionPoint.text = $"{lastCharacter.actionCount}";
		APBar.Set(lastCharacter.actionCount / 4.0f);
		energy.text = $"{lastCharacter.currentEnergy}/{lastCharacter.characterDefinition.maxEnergy}";
		energyBar.Set(lastCharacter.currentEnergy / (float)lastCharacter.characterDefinition.maxEnergy);
		movement.text = $"{lastCharacter.currentMovement}";
		movementBar.Set(lastCharacter.currentMovement / (float)lastCharacter.movementMax);
		toughness.text = $"{lastCharacter.toughness}";
		evasion.text = $"{lastCharacter.characterDefinition.evasion}";
	}

	public void UpdateStatusEffects()
	{
		if (lastCharacter == null)
		{
			return;
		}
		statusEffectDisplayGroup.Set(lastCharacter);
	}
}
