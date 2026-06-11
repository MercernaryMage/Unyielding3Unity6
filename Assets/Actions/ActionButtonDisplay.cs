using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static ActionPattern;

public class ActionButtonDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public Image icon;

	public TextMeshProUGUI actionCost;
	public TextMeshProUGUI energyCost;
	public TextMeshProUGUI movementCost;

	public GameObject actionObject;
	public GameObject energyObject;
	public GameObject movementObject;

	public GameObject disabledGraphic;

	public List<GameObject> charges;

	ActionPattern pattern;

	Character owningCharacter;

	Item owningItem;

	int targetedActionIndex;

	string reason = "";

	bool needsExit = false;

	public string uniqueName = "";

	public ActionToolTip actionToolTip;
	bool usable;

    public void Set(Character c, ActionPattern p, Item i, bool u, string unusableReason, int targetedActionIndex)
	{
		owningCharacter = c;
		pattern = p;
		owningItem = i;
		uniqueName = p.uniqueName;
		usable = u;
		this.targetedActionIndex = targetedActionIndex;
		if (p.cost.actions > 0)
		{
			actionObject.SetActive(true);
			actionCost.text = p.cost.actions.ToString();
		}
		if (p.cost.energy > 0)
		{
			energyObject.SetActive(true);
			energyCost.text = p.cost.energy.ToString();
		}
		if (p.cost.movement > 0)
		{
			movementObject.SetActive(true);
			movementCost.text = p.cost.movement.ToString();
		}

		icon.sprite = ItemRepository.Instance.GetSprite(i.itemDefinition.itemImage);

		for (int j = 0; j < p.chargesTaken; ++j)
		{
			charges[j].SetActive(true);
		}

		if (!usable)
		{
			disabledGraphic.SetActive(true);
			reason = unusableReason;
		}
	}

	public void Click()
	{
		if (!usable)
		{
			return;
		}
		if (ActionController.Instance.running)
		{
			return;
		}
		MovementController.Instance.HideMovement();
		if (pattern.attack)
		{
			if (owningItem.itemDefinition.keywords.ContainsIgnoreCase("Step"))
			{
				ActionController.Instance.BeginStep(owningCharacter, owningItem, pattern);
			}
			else
			{
				ActionController.Instance.ShowAttackableTiles(owningCharacter, owningItem, pattern);
			}
		}
		else if (pattern.useInstantAction)
		{
			if (owningCharacter.GetComponent<UseItemsOnOthersTrait>() == null || pattern.cannotBeUsedOnOthers)
			{
				owningItem.used = true;
				ActionTypes.DoInstantAction(owningCharacter, pattern);
				HeroDisplayRouter.Instance.mainDisplay.UpdateWithLastCharacter();
			}
			else
			{
				ActionPattern fakePattern = new ActionPattern();
				fakePattern.range = 1;
				fakePattern.useTargetedAction = true;
				fakePattern.targetedAction = new TargetedAction();
				fakePattern.targetedAction.actionName = "ExecuteInstantActionOnTarget";
				fakePattern.targetedAction.disqualiferFunction = "DiqaulifyInstantActionOnTarget";
				fakePattern.storedPattern = pattern;
				ActionController.Instance.ShowAttackableTiles(owningCharacter, owningItem, fakePattern);
			}
		}
		else if (pattern.useTargetedAction)
		{
			ActionController.Instance.ShowAttackableTiles(owningCharacter, owningItem, pattern);
		}
		else
		{
			ActionController.Instance.DoSimpleAction(owningCharacter, owningItem, pattern);
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		needsExit = true;
		//if (!string.IsNullOrEmpty(pattern.actionDescription) || !string.IsNullOrEmpty(pattern.actionDescriptionFunction))
		{
			actionToolTip.gameObject.SetActive(true);
			actionToolTip.Set(owningCharacter, pattern, reason, owningItem.itemDefinition);
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (needsExit)
		{
			actionToolTip.gameObject.SetActive(false);
			needsExit = false;
		}
	}
}
