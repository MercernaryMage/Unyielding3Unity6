using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class AdvantageSelectionPane : SceneSingleton<AdvantageSelectionPane>
{
	public GameObject WeaponSlotDisplayPrefab;
	public GameObject AdvantageSlotDisplayPrefab;
	public GameObject content;
	public Transform targetWeapon;
	public Transform targetAdvantage;

	List<GameObject> createdObjects = new List<GameObject>();

	WeaponSlot slot;
	ItemScriptableObject passedItem;

	void SetDisplay()
	{
		content.SetActive(true);

		foreach (GameObject obj in createdObjects)
		{
			Destroy(obj);
		}
		createdObjects.Clear();
	}

	public void Set(WeaponSlotType weaponSlotType, WeaponSlot s, ItemScriptableObject i)
	{
		passedItem = i;
		slot = s;
		SetDisplay();

		foreach (ItemScriptableObject item in PersistenceManager.Instance.unlockedItems)
		{
			if (!item.displayable)
			{
				continue;
			}
			if (!item.weapon)
			{
				continue;
			}
			if (item.slotType != weaponSlotType)
			{
				continue;
			}
			if (IsWeaponEqipped(CharacterEditor.Instance.currentCharacter, item))
			{
				continue;
			}
			GameObject obj = Instantiate(WeaponSlotDisplayPrefab);
			SlotDisplay weaponDisplay = obj.GetComponent<SlotDisplay>();
			weaponDisplay.Set(weaponSlotType, s, item);
			obj.transform.SetParent(targetWeapon);
			createdObjects.Add(obj);
		}
	}

	public void Set()
	{
		SetDisplay();
		foreach (ItemScriptableObject item in PersistenceManager.Instance.unlockedItems)
		{
			if (!item.displayable)
			{
				continue;
			}
			if (item.weapon)
			{
				continue;
			}
			if (CharacterEditor.Instance.currentCharacter.advantages.Contains(item))
			{
				continue;
			}
			DisplayItem(item);
		}
		Canvas.ForceUpdateCanvases();
		LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)targetAdvantage.transform);
	}

	bool IsWeaponEqipped(StorageCharacter c, ItemScriptableObject item)
	{
		for (int i = 0; i < c.slots.Length; ++i)
		{
			if (c.slots[i].weapon != null && c.slots[i].weapon.itemDefinition == item)
			{
				return true;
			}
		}
		return false;
	}

	void DisplayItem(ItemScriptableObject item)
	{
		GameObject obj = Instantiate(AdvantageSlotDisplayPrefab);
		InventoryItemDisplay itemDisplay = obj.GetComponent<InventoryItemDisplay>();
		itemDisplay.Set(item, false);
		obj.transform.SetParent(targetAdvantage);
		createdObjects.Add(obj);
	}

	public void Hide()
	{
		content.SetActive(false);
	}

	public void WeaponDisplayClicked(ItemScriptableObject itemScriptableObject, Item item)
	{
		Item newItem = new Item();
		slot.weapon = newItem;
		newItem.Init(itemScriptableObject);
		Hide();
		CharacterEditor.Instance.ResetToBase();
	}

	public void RemoveGameObject(GameObject obj)
	{
		createdObjects.Remove(obj);
		Destroy(obj);
	}

	public void HandleItemReturn(ItemScriptableObject item)
	{
		if (item.weapon)
		{
			return;
		}
		DisplayItem(item);
	}
}
