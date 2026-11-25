using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInspectorTraitDisplayGroup : MonoBehaviour
{
    public GameObject elementPrefab;
    public Transform target;

	List<GameObject> createdObjects = new List<GameObject>();

	Sprite traitBackground;
	Sprite traitDivider;
	bool overrideTextColors;
	Color bodyColor;
	Color titleColor;

	public void SetSprites(Sprite background, Sprite divider)
	{
		traitBackground = background;
		traitDivider = divider;
	}

	public void SetTextColors(bool overrideColors, Color body, Color title)
	{
		overrideTextColors = overrideColors;
		bodyColor = body;
		titleColor = title;
	}

	public void ClearTraits()
	{
		foreach (GameObject obj in createdObjects)
		{
			Destroy(obj);
		}
		createdObjects.Clear();
	}

	public void Set(List<TraitScriptableObject> traits)
	{
		foreach (TraitScriptableObject trait in traits)
		{
			if (trait.invisible)
			{
				continue;
			}
			GameObject obj = Instantiate(elementPrefab);
			TextSetter textSetter = obj.GetComponent<TextSetter>();
			textSetter.Set(trait.displayName, trait.description);
			if (overrideTextColors)
			{
				textSetter.bodyText.color = bodyColor;
				textSetter.titleText.color = titleColor;
			}
			ApplySprites(obj);
			obj.transform.SetParent(target);
			obj.transform.localScale = Vector3.one;
			createdObjects.Add(obj);
		}
	}

	void ApplySprites(GameObject obj)
	{
		if (traitBackground == null)
		{
			return;
		}
		foreach (Image image in obj.GetComponentsInChildren<Image>(true))
		{
			image.sprite = image.gameObject == obj ? traitBackground : traitDivider;
		}
	}
}
