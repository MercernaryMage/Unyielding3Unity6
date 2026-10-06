using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static CardDisplay;

public class CardDisplay : MonoBehaviour
{
	public enum CardType
	{
		Card,
		Reaction,
		AggravatedReaction
	}

	public GameObject content;
	public TextMeshProUGUI cardName;
	public GameObject cardDisplayItemPrefab;
	public GameObject target;
	public GameObject dismissButton;
	public GameObject cardBackground;
	public GameObject reactionBackground;
	public GameObject aggravatedBackground;

	bool dismissed = false;

	List<GameObject> createdObjects = new List<GameObject>();

	void StartCard()
	{
		FadeLerp fadeOutLerp = content.GetComponent<FadeLerp>();
		if (fadeOutLerp)
		{
			Destroy(fadeOutLerp);
		}

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
	}

	public void ShowCard(CardScriptableObject cardScriptableObject, bool showDismiss, CardType cardType)
	{
		aggravatedBackground.SetActive(cardType == CardType.AggravatedReaction);
		reactionBackground.SetActive(cardType == CardType.Reaction);
		cardBackground.SetActive(cardType == CardType.Card);

		StartCard();

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
				if (cardInstruction.instructionWords.Contains("<u>"))
				{
					obj.AddComponent<UnderlinedTextHover>();
				}
				createdObjects.Add(obj);
			}
			if (cardInstruction.instruction == InstructionType.Grid)
			{
				DisplayGrid.Instance.SetIndex(index);
			}
			++index;
		}
		FadeLerp lerp = target.AddComponent<FadeLerp>();
		lerp.BasicFadeIn();
		lerp.Init();

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
		cardBackground.SetActive(true);
		aggravatedBackground.SetActive(false);
		reactionBackground.SetActive(false);

		StartCard();
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

	public void Dismiss(bool fly)
	{
		if (!dismissed)
		{
			dismissed = true;
			if (fly)
			{
				GameObject obj = Instantiate(gameObject);
				obj.transform.SetParent(transform.parent, false);

				ScaleLerp scaleLerp = obj.AddComponent<ScaleLerp>();
				scaleLerp.startScale = 1;
				scaleLerp.endScale = .1f;
				scaleLerp.runTime = .75f;
				scaleLerp.Init();

				GlobalPositionLerp positionLerp = obj.AddComponent<GlobalPositionLerp>();
				positionLerp.p0 = obj.transform.position;
				positionLerp.p1 = CombatLogControl.Instance.UIPoint.position;
				positionLerp.runTime = .75f;
				positionLerp.destroy = true;
				positionLerp.Init();

				DismissActual();
			}
			else
			{
				FadeLerp lerp = content.AddComponent<FadeLerp>();
				lerp.BasicFadeOut();
				lerp.callbackFunction = DismissActual;
				lerp.Init();
			}
		}
	}

	public void DismissActual()
	{
		AICardDisplay.Instance.isShowing = false;
		content.SetActive(false);
	}
}
