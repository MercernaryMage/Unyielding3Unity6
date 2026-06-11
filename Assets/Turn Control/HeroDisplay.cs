using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.TextCore.Text;
using System.Linq;
using UnityEngine.UI;

public class HeroDisplay : MonoBehaviour
{
	public bool isPrimary = true;
	public GameObject actionButtonPrefab;

	public GameObject content;
    public TextMeshProUGUI characterName;
	public TextMeshProUGUI hp;
	public TextMeshProUGUI determination;
	public TextMeshProUGUI armor;
	public TextMeshProUGUI evasion;
	public TextMeshProUGUI energy;

	public TextMeshProUGUI movementAmountText;
	public TextMeshProUGUI actionAmountText;

	public TextMeshProUGUI prowessText;
	public TextMeshProUGUI cunningText;

	public GameObject actionContent;

	List<ActionButtonDisplay> createdObjects = new List<ActionButtonDisplay>();

	public Character lastCharacter;

	public bool showing;

	public GameObject stunnedLabel;

	public Transform traitTarget;
	public GameObject traitPrefab;
	List<GameObject> createdTraits = new List<GameObject>();

	public void Hide(bool hideMovement)
	{
		content.SetActive(false);
		if (hideMovement)
		{
			MovementController.Instance.HideMovement();
		}
		showing = false;
	}

	public void Show()
	{
		content.SetActive(true);
		showing = true;
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

	public void Set(Character character)
	{
		lastCharacter = character;
		foreach (ActionButtonDisplay obj in createdObjects)
		{
			Destroy(obj.gameObject);
		}
		createdObjects.Clear();
		characterName.text = character.displayName;
		hp.text = $"{character.currentHP}/{character.maxHP}";
		determination.text = $"{character.storageCharacter.currentDetermination}";
		armor.text = $"{character.armor}";
		movementAmountText.text = $"{character.currentMovement}M";
		actionAmountText.text = $"{character.actionCount}A";
		prowessText.text = character.characterDefinition.prowess.ToString();
		cunningText.text = character.characterDefinition.cunning.ToString();
		evasion.text = character.characterDefinition.evasion.ToString();
		energy.text = $"{character.currentEnergy}/{character.characterDefinition.maxEnergy}";

		foreach (GameObject obj in createdTraits)
		{
			Destroy(obj);
		}
		createdTraits.Clear();
		foreach (TraitScriptableObject trait in character.characterDefinition.traits)
		{
			GameObject newTrait = Instantiate(traitPrefab);
			TraitDisplay display = newTrait.GetComponent<TraitDisplay>();
			display.Set(trait);
			newTrait.transform.SetParent(traitTarget);
			createdTraits.Add(newTrait);
		}
		Canvas.ForceUpdateCanvases();
		LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)traitTarget.transform);

		if (!isPrimary)
		{
			return;
		}

		if (character.GetComponent<Stun>() || character.GetComponent<Downed>())
		{
			stunnedLabel.SetActive(true);
		}
		else
		{
			stunnedLabel.SetActive(false);
			List<Item> allItems = new List<Item>(character.storageCharacter.equipment);
			allItems.AddRange(character.temporaryItems);
			foreach (Item item in allItems)
			{
				for (int i = 0; i < item.itemDefinition.actions.Count; ++i)
				{
					ActionPattern pattern = item.itemDefinition.actions[i];
					if (!IsActionDisplayable(character, item, pattern))
					{
						continue;
					}
					if (pattern.uniqueName != "" && createdObjects.Any(o => o.uniqueName == pattern.uniqueName))
					{
						continue;
					}
					GameObject obj = Instantiate(actionButtonPrefab);
					obj.transform.SetParent(actionContent.transform);
					Tuple<bool, string> usable = IsActionUsable(character, item, pattern);
					ActionButtonDisplay display = obj.GetComponent<ActionButtonDisplay>();
					obj.GetComponent<ActionButtonDisplay>().Set(character, pattern, item, usable.Item1, usable.Item2, i);
					createdObjects.Add(display);
				}
			}
		}		
	}

	public static bool CanAffordAction(Character c, ActionPattern a)
	{
		if (a.cost.actions > 0)
		{
			return c.actionCount >= a.cost.actions;
		}
		
		if (a.cost.movement > 0)
		{
			return c.currentMovement >= a.cost.movement;
		}
		return true;
	}

	public static Tuple<bool, string> IsActionUsable(Character c, Item i, ActionPattern a)
	{		
		if (i.used)
		{
			return new Tuple<bool, string>(false, "Item was used this turn");
		}
		if (a.keywords.ContainsIgnoreCase("Load") && i.loaded == true)
		{
			return new Tuple<bool, string>(false, "Item is loaded");
		}
		if (a.keywords.ContainsIgnoreCase("Loading") && i.loaded == false)
		{
			return new Tuple<bool, string>(false, "Item needs to reload");
		}
		if (!CanAffordAction(c, a))
		{
			return new Tuple<bool, string>(false, "Not enough actions");
		}
		if (a.chargesTaken > 0 && a.chargesTaken > i.charges)
		{
			return new Tuple<bool, string>(false, "Not enough charges");
		}
		if (a.useInstantAction)
		{
			if (c.gameObject.GetComponent<UseItemsOnOthersTrait>() == null || a.cannotBeUsedOnOthers)
			{
				Tuple<bool, string> disqualifier = ActionTypes.DoDisqualifierForInstantAction(c, a);
				if (!disqualifier.Item1)
				{
					return disqualifier;
				}
			}
		}
        if (a.attack && a.keywords.Contains("Ranged") && c.GetComponent<Jammed>() != null)
        {
			return new Tuple<bool, string>(false, "Jammed");
		}
		return new Tuple<bool, string>(true, ""); ;
	}

	public static bool IsActionDisplayable(Character c, Item i, ActionPattern a)
	{
		if (!string.IsNullOrEmpty(a.disqualifierFunc))
		{
			if (!ActionTypes.DoDisqualifierForActionButton(c, a, i))
			{
				return false;
			}
		}
		if (a.keywords.ContainsIgnoreCase("LoadingOnce") && i.loaded == false)
		{
			return false;
		}
		

		return true;
	}
}
