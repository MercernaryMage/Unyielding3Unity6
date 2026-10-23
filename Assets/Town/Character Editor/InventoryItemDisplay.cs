using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItemDisplay : MonoBehaviour, IPointerClickHandler
{
    public Image itemIcon;
    public TextMeshProUGUI itemName;
	public TextMeshProUGUI itemDescription;
	public TextMeshProUGUI itemCost;
	public GameObject deleteButton;
	bool inInventory;
	ItemScriptableObject item;

	public void Set(ItemScriptableObject i, bool inventory = true)
	{
		item = i;
		inInventory = inventory;
		if (!inInventory)
		{
			deleteButton.SetActive(false);
		}
		if (PersistenceManager.Instance.GetFlag("AdvantagesLocked") == true)
		{
			deleteButton.SetActive(false);
		}
		itemIcon.sprite = item.itemImage;
		itemName.text = item.displayName;
		itemCost.text = item.weight.ToString();
		string charges = "";
		if (item.charges > 0)
		{
			charges = $" {item.charges} {Util.Pluralize(item.charges, "charge")}";
			itemDescription.text = $"{charges}\n{item.actions[0].actionDescription}";
		}
		else
		{
			itemDescription.text = $"{item.actions[0].actionDescription}";
		}

	}

	public void Remove()
	{
		CharacterEditor.Instance.RemoveItemFromInventory(item);
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (inInventory) 
		{
			return;
		}
		CharacterEditor.Instance.AddItemToInventory(item);
		AdvantageSelectionPane.Instance.RemoveGameObject(gameObject);
	}
}
