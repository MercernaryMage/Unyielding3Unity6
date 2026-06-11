using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
	public GameObject content;
	public TextMeshProUGUI cardName;
	public GameObject cardDisplayItemPrefab;
	public GameObject target;
	public GameObject dismissButton;

	bool dismissed = false;

	List<GameObject> createdObjects = new List<GameObject>();

	public void ShowCard(CardScriptableObject cardScriptableObject, bool showDismiss)
	{
		FadeLerp lerp = content.AddComponent<FadeLerp>();
		lerp.BasicFadeIn();
		lerp.Init();
		dismissed = false;

		foreach (GameObject obj in createdObjects)
		{
			obj.transform.SetParent(null);
			Destroy(obj);
		}
		createdObjects.Clear();
		content.SetActive(true);
		cardName.text = cardScriptableObject.cardDisplayName;
		DisplayGrid.Instance.Hide();
		List<CardInstruction> instructions = (List<CardInstruction>)System.Type.GetType(cardScriptableObject.className)
			.GetMethod("GetCardInstructions", BindingFlags.Public | BindingFlags.Static)
			.Invoke(null, new object[] { cardScriptableObject });
		int index = 0;
		foreach (CardInstruction cardInstruction in instructions)
		{
			if (cardInstruction.instruction == InstructionType.String)
			{
				GameObject obj = Instantiate(cardDisplayItemPrefab);
				obj.transform.SetParent(target.transform);
				obj.GetComponent<TextMeshProUGUI>().text = cardInstruction.instructionWords;
				createdObjects.Add(obj);
			}
			if (cardInstruction.instruction == InstructionType.Grid)
			{
				DisplayGrid.Instance.SetIndex(index);
			}
			++index;
		}
		dismissButton.SetActive(showDismiss);
		dismissButton.transform.SetSiblingIndex(10000);
		LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)target.transform);
	}

	public void ShowFakeCard(string title, string description)
	{
		foreach (GameObject obj in createdObjects)
		{
			obj.transform.SetParent(null);
			Destroy(obj);
		}
		createdObjects.Clear();
		content.SetActive(true);
		cardName.text = title;
		DisplayGrid.Instance.Hide();
		GameObject display = Instantiate(cardDisplayItemPrefab);
		display.transform.SetParent(target.transform);
		display.GetComponent<TextMeshProUGUI>().text = description;
		createdObjects.Add(display);
		dismissButton.SetActive(true);
		dismissButton.transform.SetSiblingIndex(10000);
		LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)target.transform);
	}

	public void Dismiss()
	{
		if (!dismissed)
		{
			FadeLerp lerp = content.AddComponent<FadeLerp>();
			lerp.BasicFadeOut();
			lerp.callbackFunction = DismissActual;
			lerp.Init();
			dismissed = true;
		}
	}

	public void DismissActual()
	{
		AICardDisplay.Instance.isShowing = false;
		content.SetActive(false);
	}
}
