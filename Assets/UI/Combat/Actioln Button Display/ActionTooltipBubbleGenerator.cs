using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ActionTooltipBubbleGenerator : MonoBehaviour
{
    public GameObject bubblePrefab;
    public float width;
	public float minHorizontalSpacing;
	public float verticalSpacing;
	public float lineHeight = 40;

	public Transform overallTarget;

	List<GameObject> createdObjecs = new List<GameObject>();
	GameObject previousOverObject = null;

	public void Create(List<string> values)
	{
		values.Reverse();
		foreach (GameObject obj in createdObjecs)
		{
			Destroy(obj);
		}
		createdObjecs.Clear();
		if (previousOverObject != null)
		{
			Destroy(previousOverObject);
		}
		GameObject createdOverObject = new GameObject();
		createdOverObject.AddComponent<RectTransform>();
		previousOverObject = createdOverObject;
		createdOverObject.transform.SetParent(overallTarget, false);
		List<GameObject> createdBubbles = new List<GameObject>();
		foreach (string str in values)
		{
			GameObject obj = Instantiate(bubblePrefab);
			obj.GetComponent<ActionButtonTooiltipKeyword>().Set(str);
			obj.name = str;
			createdBubbles.Add(obj);
			obj.transform.SetParent(createdOverObject.transform, false);
			createdObjecs.Add(obj);
		}

		Canvas.ForceUpdateCanvases();
		foreach (GameObject obj in createdBubbles)
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)obj.transform);
		}
		float y = 0;
		List<GameObject> lineBubbles = new List<GameObject>();
		while (createdBubbles.Count > 0)
		{
			float sum = GetTotalLength(lineBubbles);
			float fit = minHorizontalSpacing * 2 + sum + Mathf.Max(0, lineBubbles.Count - 1) * minHorizontalSpacing;
			if (fit + ((RectTransform)createdBubbles[0].transform).rect.width + minHorizontalSpacing < width)
			{
				lineBubbles.Add(createdBubbles[0]);
				createdBubbles.RemoveAt(0);
				SetHorizontalSpacing(lineBubbles, y);
			}
			else
			{
				if (lineBubbles.Count == 0)
				{
					Debug.LogError($"INFITE LOOP ABORTED FROM {createdBubbles[0].GetComponent<ActionButtonTooiltipKeyword>().textField.text}");
					return;
				}
				y += lineHeight;
				lineBubbles.Clear();
			}
		}
		previousOverObject.transform.localPosition = Vector3.zero;
		previousOverObject.GetComponent<RectTransform>().sizeDelta = new Vector2(100, y + lineHeight);
	}

	float GetTotalLength(List<GameObject> lineObjects)
	{
		float sum = 0;
		foreach (GameObject obj in lineObjects)
		{
			sum += ((RectTransform)obj.transform).rect.width;
		}
		return sum;
	}

	void SetHorizontalSpacing(List<GameObject> lineObjects, float y)
	{
		float sum = GetTotalLength(lineObjects);
		float paddingRemaining = width - sum;
		int units = lineObjects.Count + 1;
		float unitPadding = paddingRemaining / units;
		float run = -width / 2 + unitPadding;
		foreach (GameObject obj in lineObjects)
		{
			float tempWidth = ((RectTransform)obj.transform).rect.width;
			run += tempWidth / 2;
			obj.transform.localPosition = new Vector3(run, y, 0);
			run += tempWidth / 2;
			run += unitPadding;
		}
	}
}
