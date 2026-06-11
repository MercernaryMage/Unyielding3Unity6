using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class CharacterEditor : SceneSingleton<CharacterEditor>
{
    public List<CharacterEditorCharacter> characterTargets;

	public Image characterPortrait;
	public TextMeshProUGUI characterName;
	public TextMeshProUGUI hp;
	public TextMeshProUGUI energy;
	public TextMeshProUGUI movement;
	public TextMeshProUGUI toughness;
	public TextMeshProUGUI armor;
	public TextMeshProUGUI evasion;

	public List<SubclassDisplay> subclasses;
	public List<SlotDisplay> slots;

	public GameObject advantagePrefab;
	public List<Transform> advantageTargets;
	List<GameObject> createdAdvantages = new List<GameObject>();
	public StorageCharacter currentCharacter;
	public GameObject subclassDispaly;

	public GameObject traitDisplayPrefab;
	public Transform traitTarget;
	List<GameObject> createdTraits = new List<GameObject>();

	public GameObject addButton;

	private void Start()
	{
		for (int i = 0; i < PersistenceManager.Instance.currentTeam.Count; i++)
		{
			characterTargets[i].Set(PersistenceManager.Instance.currentTeam[i]);
		}
		SetToCharacter(PersistenceManager.Instance.currentTeam[0]);
		addButton.SetActive(!PersistenceManager.Instance.GetFlag("AdvantagesLocked") == true);
	}

	public void SetToCharacter(StorageCharacter storageCharacter)
	{
		subclassDispaly.SetActive(!PersistenceManager.Instance.GetFlag("WeaponSlotsLocked"));
		currentCharacter = storageCharacter;
		characterPortrait.sprite = storageCharacter.characterDefintion.bigPortrait;
		characterName.text = storageCharacter.characterDefintion.displayName;

		hp.text = storageCharacter.characterDefintion.maxHP.ToString();
		energy.text = storageCharacter.characterDefintion.maxEnergy.ToString();
		movement.text = storageCharacter.characterDefintion.movement.ToString();
		toughness.text = storageCharacter.characterDefintion.toughness.ToString();
		armor.text = storageCharacter.characterDefintion.armor.ToString();
		evasion.text = storageCharacter.characterDefintion.evasion.ToString();

		for (int i = 0; i < storageCharacter.characterDefintion.subclasses.Count; ++i)
		{
			subclasses[i].Set(storageCharacter, i);
		}

		for (int i = 0; i < slots.Count; ++i)
		{
			if (i >= storageCharacter.characterDefintion.slots.Count)
			{
				slots[i].gameObject.SetActive(false);
				continue;
			}
			slots[i].gameObject.SetActive(true);
			slots[i].Set(storageCharacter.characterDefintion.slots[i], storageCharacter.slots[i], storageCharacter.slots[i].weapon);
		}

		foreach (GameObject obj in createdAdvantages)
		{
			Destroy(obj);
		}
		createdAdvantages.Clear();

		foreach (ItemScriptableObject item in storageCharacter.advantages)
		{
			GameObject obj = Instantiate(advantagePrefab);
			createdAdvantages.Add(obj);
			ShuffleItemDisplay();
			InventoryItemDisplay inventoryItemDisplay = obj.GetComponent<InventoryItemDisplay>();
			inventoryItemDisplay.Set(item);
		}

		foreach (GameObject obj in createdTraits)
		{
			Destroy(obj);
		}
		createdTraits.Clear();

		if (!PersistenceManager.Instance.GetFlag("WeaponSlotsLocked"))
		{
			foreach (TraitScriptableObject traitScriptableObject in storageCharacter.characterDefintion.traits)
			{
				GameObject obj = Instantiate(traitDisplayPrefab);
				obj.GetComponent<TraitDisplay>().Set(traitScriptableObject);
				obj.transform.SetParent(traitTarget);
				createdTraits.Add(obj);
			}
			Canvas.ForceUpdateCanvases();
			LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)traitTarget.transform);
			LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)advantageTargets[0].parent.transform);
		}
	}

	public void ResetToBase()
	{
		SetToCharacter(currentCharacter);
	}

	public void AddAdvantageClicked()
	{
		AdvantageSelectionPane.Instance.Set();
	}

	public void AddItemToInventory(ItemScriptableObject itemScriptableObject)
	{
		currentCharacter.advantages.Add(itemScriptableObject);
		ResetToBase();
	}

	public void RemoveItemFromInventory(ItemScriptableObject itemScriptableObject)
	{
		currentCharacter.advantages.Remove(itemScriptableObject);
		AdvantageSelectionPane.Instance.HandleItemReturn(itemScriptableObject);
		ResetToBase();
	}

	public void ShuffleItemDisplay()
	{
		for (int i = 0; i < createdAdvantages.Count; ++i)
		{
			int index = i / 3;
			createdAdvantages[i].transform.SetParent(advantageTargets[index]);
		}
	}

	private void Update()
	{
		if (Input.GetMouseButtonUp(0))
		{
			PointerEventData pointerData = new PointerEventData(EventSystem.current);
			pointerData.position = Input.mousePosition;

			List<RaycastResult> results = new List<RaycastResult>();
			EventSystem.current.RaycastAll(pointerData, results);
			bool keepOpen = false;
			foreach (RaycastResult result in results)
			{
				if (UTag.HasTag(result.gameObject, "KeepScreenOpen"))
				{
					keepOpen = true;
					break;
				}
			}
			if (!keepOpen)
			{
				AdvantageSelectionPane.Instance.Hide();
			}
		}
	}
}
