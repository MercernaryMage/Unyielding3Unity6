using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class GenericTooltip : SceneSingleton<GenericTooltip>
{
	public GameObject tooltipObject;
	public TextMeshProUGUI title;
	public TextMeshProUGUI body;
	public float fadeSpeed = 4f;

	CanvasGroup canvasGroup;
	float targetAlpha = 0f;

	bool block = false;

	void Awake()
	{
		canvasGroup = tooltipObject.GetComponent<CanvasGroup>();
		if (canvasGroup == null)
		{
			canvasGroup = tooltipObject.AddComponent<CanvasGroup>();
		}
		//purely informational, never intercept the mouse
		canvasGroup.interactable = false;
		canvasGroup.blocksRaycasts = false;
		canvasGroup.alpha = 0f;
		tooltipObject.SetActive(true);
	}

	public void Set(string titleText, string bodyText)
	{
		title.text = titleText;
		body.text = bodyText;
		targetAlpha = 1f;
	}

	public void Hide()
	{
		targetAlpha = 0f;
	}

	public void Block()
	{
		block = true;
	}

	void Update()
	{
		if (!EventSystem.current.IsPointerOverGameObject() && !block)
		{
			targetAlpha = 0f;
		}
		block = false;
		canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, targetAlpha, fadeSpeed * Time.deltaTime);
	}
}
